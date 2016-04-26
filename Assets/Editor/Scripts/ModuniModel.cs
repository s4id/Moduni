//
//  ModuniModel.cs
//
//  Author:
//       Moduni contributors
//
//  Copyright (c) 2016 Moduni contributors
//
//  This file is part of Moduni.
//
//  Moduni is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Moduni
{
    public delegate void GenericEventHandler<TSender,TArg>(TSender sender,TArg arg);

    public class ModuniModel : IModuniModel
    {
        private ModuleFactory moduleFactory;
        private List<Tuple<IRepositoryManager,IModule>> modules;
        private List<IModule> projectModules;
        private ISourceControlRepository projectRepository;
        private List<IRepositoryManager> repositoriesManagers;
        private SourceControlRepositoryFactory repositoryFactory;
        private RepositoryManagerFactory repositoryManagerFactory;
        private SettingsManager settingsManager;

        public ModuniModel()
        {
            ServicePointManager.ServerCertificateValidationCallback += 
            (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                if (sslPolicyErrors != SslPolicyErrors.None)
                {
                    foreach (X509ChainStatus chainStatus in chain.ChainStatus)
                    {
                        if (chainStatus.Status == X509ChainStatusFlags.UntrustedRoot)
                            return false;
                    }
                }
                return true;
            };

            this.moduleFactory = new ModuleFactory();
            this.repositoryFactory = new SourceControlRepositoryFactory();
            this.repositoryManagerFactory = new RepositoryManagerFactory();

            this.settingsManager = new SettingsManager();
            this.settingsManager.Load();

            this.DiscoverSourceControlProjectRepository();
            this.DiscoverProjectModules();
            this.repositoriesManagers = new List<IRepositoryManager>();
            this.modules = new List<Tuple<IRepositoryManager,IModule>>();
            this.CreateRepositoriesManagers();
        }

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModulesUpdatedHandler OnModulesUpdated;
        public event OnProjectModulesUpdatedHandler OnProjectModulesUpdated;
        public event OnRepositoriesManagersUpdatedHandler OnRepositoriesManagersUpdated;
        public event OnRepositoriesManagersSettingsUpdatedHandler OnRepositoriesManagersSettingsUpdated;

        public DeveloperSettings DeveloperSettings
        {
            get
            {
                return this.settingsManager.ModuniSettings.DeveloperSettings;
            }
        }

        public ModuleFactory ModuleFactory
        {
            get
            {
                return this.moduleFactory;
            }
        }

        public IEnumerable<Tuple<IRepositoryManager, IModule>> Modules
        {
            get
            {
                return this.modules;
            }
        }

        public IEnumerable<IModule> ProjectModules
        {
            get
            {
                return this.projectModules;
            }
        }

        public IEnumerable<IRepositoryManager> RepositoriesManagers
        {
            get
            {
                return this.repositoriesManagers;
            }
        }

        public IEnumerable<ARepositoryManagerSettings> RepositoriesManagersSettings
        {
            get
            {
                return this.settingsManager.ModuniSettings.RepositoryManagerSettings;
            }
        }

        public void CreateModule(IRepositoryManager repositoryManager, ModuleState moduleState, Moduni.BranchVersion version)
        {
            Task<ISourceControlRepository> creationTask = repositoryManager.CreateRepository(moduleState.Name);
            creationTask.Wait();
            ISourceControlRepository remoteRepository = creationTask.Result;
            ISourceControlRepository projectRepository = remoteRepository.InitOrCloneRepository(moduleState.Path);
            IModule module = this.moduleFactory.CreateModuleFromRepository(projectRepository);
            module.CurrentState = moduleState;
            module.SaveMetadata();
            module.PublishChanges("feat(Global): initial commit of the module");
            module.PublishVersion(version);
            module.CreateBaseBranches();
            this.projectModules.Add(module);

            if (this.projectRepository != null)
            {
                this.projectRepository.AddSubmodule(module.Path, module.RepositoryURL);
                this.projectRepository.StageFile(projectRepository.RepositoryURL.Remove(projectRepository.RepositoryURL.Length - 1, 1));
            }
            this.modules.Add(new Tuple<IRepositoryManager, IModule>(repositoryManager, this.moduleFactory.CreateModuleFromRepository(remoteRepository)));

            module.CheckoutBranchVersion(new BranchVersion("master"));

            if (this.OnModulesUpdated != null)
                this.OnModulesUpdated(this.modules);
            if (this.OnProjectModulesUpdated != null)
                this.OnProjectModulesUpdated(this.projectModules);
            if (this.OnMessageTriggered != null)
                this.OnMessageTriggered(new Message(string.Format("Module ({0}) {1} created successfully !", repositoryManager.Name, module.Name), MessageType.Info));
        }

        public void DeleteModules(IEnumerable<IModule> modulesToDelete)
        {
            foreach (IModule module in modulesToDelete)
            {
                module.DisposeRepository();
                if (this.projectRepository != null)
                {
                    this.projectRepository.RemoveSubmodule(this.projectRepository.GetSubmoduleRelativePath(module.Path), false);
                }
                this.projectModules.Remove(module);
            }
            if (this.projectRepository != null)
            {
                this.projectRepository.Commit("chore(modules): delete modules");
            }
            AssetDatabase.Refresh();
            if (this.OnProjectModulesUpdated != null)
                this.OnProjectModulesUpdated(this.projectModules);
        }

        public void ImportModules(IEnumerable<IModule> modulesToImport)
        {
            IModule projectModule;
            bool hasError = false;
            bool shouldStageModule = false, shouldCommit = false;
            foreach (IModule module in modulesToImport)
            {
                projectModule = this.projectModules.Find((IModule moduleSearched) => moduleSearched.UUID == module.UUID); 
                ISourceControlRepository projectModuleRepository;
                if (projectModule == null)
                {
                    if (this.projectRepository != null)
                    {
                        projectModuleRepository = this.projectRepository.AddSubmodule(module.RepositoryURL, module.Path);
                    }
                    else
                    {
                        projectModuleRepository = module.CloneRepository(module.Path);
                    }
                    projectModule = this.moduleFactory.CreateModuleFromRepository(projectModuleRepository);
                    projectModule.CheckoutBranchVersion(module.CurrentBranchVersion);
                    this.projectModules.Add(projectModule);
                    shouldStageModule = true;
                }
                else
                {
                    if (module.CurrentBranchVersion != projectModule.CurrentBranchVersion)
                    {
                        try
                        {
                            string moduleOldPath = projectModule.Path;
                            projectModule.CheckoutBranchVersion(module.CurrentBranchVersion);
                            if (projectModule.Path != moduleOldPath)
                            {
                                this.MoveSubmodule(projectModule, moduleOldPath, projectModule.Path);
                            }
                            shouldStageModule = true;
                        }
                        catch (ModuniException e)
                        {
                            hasError = true;
                            if (this.OnMessageTriggered != null)
                                this.OnMessageTriggered(new Message(e.Message, MessageType.Error));
                        }
                    }
                    else
                    {
                        shouldStageModule = false;
                    }
                }
                if (this.projectRepository != null && shouldStageModule)
                {
                    this.projectRepository.StageFile(this.projectRepository.GetSubmoduleRelativePath(projectModule.Path));
                    shouldCommit = true;
                }
            }
            if (this.projectRepository != null && shouldCommit)
            {
                StringBuilder commitMessage = new StringBuilder("chore(modules): import modules: ");
                foreach (IModule module in modulesToImport)
                {
                    commitMessage.Append(string.Format("{0} [{1}], ", module.Name, module.CurrentBranchVersion));
                }
                commitMessage.Remove(commitMessage.Length - 2, 2);
                this.projectRepository.Commit(commitMessage.ToString());
            }
            AssetDatabase.Refresh();
            if (this.OnProjectModulesUpdated != null)
                this.OnProjectModulesUpdated(this.projectModules);
            if (!hasError)
            {
                if (this.OnMessageTriggered != null)
                    this.OnMessageTriggered(new Message("Modules imported successfully !", MessageType.Info));
            }
        }

        public void ModifyModule(IModule module, ModuleState moduleState)
        {
            bool shouldCommit = false;
            if (module.CurrentBranchVersion.ToString() == "master")
            {
                string moduleOldPath = module.Path;
                if (moduleState.Path != moduleOldPath)
                {
                    this.MoveSubmodule(module, moduleOldPath, moduleState.Path);
                }
                module.CurrentState = moduleState;
                module.SaveMetadata();
                if (module.IsDirty)
                {
                    module.PublishChanges("chore(metadata): modify the metadata of the module");
                    shouldCommit = true;
                }
                if (this.projectRepository != null && shouldCommit)
                {
                    this.projectRepository.StageFile(this.projectRepository.GetSubmoduleRelativePath(moduleState.Path));
                    this.projectRepository.Commit(string.Format("chore(modules): modify module {0} [{1}]", module.Name, module.CurrentBranchVersion));
                }

                if (moduleState.Path != moduleOldPath)
                    AssetDatabase.Refresh();

                if (this.OnMessageTriggered != null)
                    this.OnMessageTriggered(new Message(string.Format("Module modified {0} [{1}] successfully !", module.Name, module.CurrentBranchVersion), MessageType.Info));
            }
            else
            {
                throw new ModuniException("You can't modify the metadata of a module that is not currently on the branch 'master'.");
            }
        }

        public void PublishChanges(IModule modulePublished, string commitMessage)
        {
            modulePublished.PublishChanges(commitMessage);
        }

        public void PublishVersion(IModule modulePublished, string versionMessage, BranchVersion newVersion)
        {
            modulePublished.PublishVersion(newVersion);
        }

        public void UpdateSettings(DeveloperSettings developerSettings, IEnumerable<ARepositoryManagerSettings> repositoriesManagersSettings, bool isProjectOnlySettings)
        {
            this.settingsManager.ModuniSettings.DeveloperSettings = developerSettings;
            this.settingsManager.ModuniSettings.RepositoryManagerSettings = new List<ARepositoryManagerSettings>(repositoriesManagersSettings);
            if (isProjectOnlySettings)
                this.settingsManager.SaveForProject();
            else
                this.settingsManager.SaveForProjectAndEditor();
            if (OnRepositoriesManagersSettingsUpdated != null)
                OnRepositoriesManagersSettingsUpdated(this.settingsManager.ModuniSettings.RepositoryManagerSettings);
            this.CreateRepositoriesManagers();
        }

        void CreateRepositoriesManagers()
        {
            this.repositoriesManagers.Clear();
            IRepositoryManager repositoryManager;
            Task<IEnumerable<ISourceControlRepository>> repositoriesRequest;
            IModule module;
            foreach (ARepositoryManagerSettings repositoryManagerSettings in this.settingsManager.ModuniSettings.RepositoryManagerSettings)
            {
                repositoryManager = this.repositoryManagerFactory.CreateRepositoryManagerFromSettings(repositoryManagerSettings);
                this.repositoriesManagers.Add(repositoryManager);

                try
                {
                    repositoriesRequest = repositoryManager.GetRepositories();
                    repositoriesRequest.Wait();
                    foreach (ISourceControlRepository repository in repositoriesRequest.Result)
                    {
                        module = this.moduleFactory.CreateModuleFromRepository(repository);
                        this.modules.Add(new Tuple<IRepositoryManager, IModule>(repositoryManager, module));
                    }
                }
                catch (AggregateException e)
                {
                    Exception moduniException = e.InnerExceptions.FirstOrDefault((Exception exceptionSearched) => exceptionSearched.GetType() == typeof(ModuniException));
                    if (moduniException != null)
                    {
                        Debug.LogError(moduniException.ToString());
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            if (this.OnModulesUpdated != null)
                this.OnModulesUpdated(this.modules);
            if (this.OnRepositoriesManagersUpdated != null)
                this.OnRepositoriesManagersUpdated(this.repositoriesManagers);
        }

        void DiscoverProjectModules()
        {
            this.projectModules = new List<IModule>();
            string[] gitFiles = Directory.GetFiles(Application.dataPath, ".git", SearchOption.AllDirectories);
            string repositoryDirectory;
            foreach (string gitFile in gitFiles)
            {
                repositoryDirectory = new FileInfo(gitFile).Directory.FullName;
                if (Directory.GetFileSystemEntries(repositoryDirectory).Any((string file) => file.Contains(Module.ModuleMetadataFilename)))
                {
                    ISourceControlRepository sourceControlRepository = this.repositoryFactory.CreateRepository(repositoryDirectory);
                    this.projectModules.Add(this.moduleFactory.CreateModuleFromRepository(sourceControlRepository));
                }
            }
        }

        void DiscoverSourceControlProjectRepository()
        {
            this.projectRepository = this.repositoryFactory.DiscoverRepository(Application.dataPath);
        }

        void MoveSubmodule(IModule module, string oldPath, string newPath)
        {
            if (this.projectRepository != null)
            {
                module.DisposeRepository();
                this.projectRepository.MoveSubmodule(this.projectRepository.GetSubmoduleRelativePath(oldPath), this.projectRepository.GetSubmoduleRelativePath(newPath));
                int moduleIndex = this.projectModules.FindIndex((IModule moduleSearched) => moduleSearched == module);
                module = this.moduleFactory.CreateModuleFromRepository(this.repositoryFactory.CreateRepository(newPath));
                this.projectModules[moduleIndex] = module;
                if (this.OnProjectModulesUpdated != null)
                    this.OnProjectModulesUpdated(this.projectModules);
            }
        }
    }
}

