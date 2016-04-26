//
//  ModulePublicationTab.cs
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

namespace Moduni.UI
{
    using System;
    using UnityEditor;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;

    public class ModulePublicationTab : ITab, IMessageTrigger
    {
        const string DefaultCommitMessage = "feat(<scope>): do something";

        private string commitMessage;
        private ModulesToggleGroup modulesToggleGroup;
        private IModuniModel moduniModel;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ModulePublicationTab(IModuniModel moduniModel, GUIStyles styles)
        {
            this.moduniModel = moduniModel;
            this.styles = styles;
            this.modulesToggleGroup = new ModulesToggleGroup(this.CreateSelectors(moduniModel.ProjectModules, moduniModel.Modules));
            this.moduniModel.OnProjectModulesUpdated += this.OnProjectModulesUpdated;
            this.commitMessage = ModulePublicationTab.DefaultCommitMessage;
        }

        public delegate void OnModuleChangesPublicationSelectedHandler(IModule moduleSelected,string commitMessage);

        public delegate void OnModuleVersionPublicationSelectedHandler(IModule moduleSelected,string commitMessage,BranchVersion newVersion);

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModuleChangesPublicationSelectedHandler OnModuleChangesPublicationSelected;
        public event OnModuleVersionPublicationSelectedHandler OnModuleVersionPublicationSelected;

        public string Name
        {
            get
            {
                return "Publication";
            }
        }

        public void Display()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (!this.moduniModel.RepositoriesManagers.Any())
            {
                EditorGUILayout.LabelField("No repositories found. Please, create a repository and modules before you can publish anything.", EditorStyles.boldLabel, null);
            }
            else if (!this.moduniModel.ProjectModules.Any())
            {
                EditorGUILayout.LabelField("No modules found in your project. Please, import or create modules before you can publish anything.", EditorStyles.boldLabel, null);
            }
            else
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Click on the name of the module you want to publish, type in the message for the publication and then select one type of publication below."), this.styles.BoldLabelStyle, GUILayout.MinWidth(500f));
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
                    {
                        this.scrollPosition = scrollView.scrollPosition;
                        this.modulesToggleGroup.Display();
                    }
                }
                EditorGUILayout.Separator();
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Type in below a message for your publication.");
                    this.commitMessage = EditorGUILayout.TextArea(this.commitMessage);
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                        }
                        IModule module = this.modulesToggleGroup.ObjectSelected;
                        if (module != null)
                        {
                            BranchVersion maxVersion = module.Versions.Where((BranchVersion branchVersion) => branchVersion.IsVersion).Max();
                            BranchVersion majorVersion = maxVersion, minorVersion = maxVersion, patchVersion = maxVersion;
                            string errorMessage = "Please type in a message for your publication.";
                            majorVersion.IncreaseMajorVersion();
                            minorVersion.IncreaseMinorVersion();
                            patchVersion.IncreasePatchVersion();

                            if (GUILayout.Button("Publish changes", GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                            {
                                if (string.IsNullOrEmpty(this.commitMessage) || this.commitMessage == ModulePublicationTab.DefaultCommitMessage)
                                {
                                    if (this.OnMessageTriggered != null)
                                        this.OnMessageTriggered(new Message(errorMessage, MessageType.Info));
                                }
                                else
                                {
                                    if (this.OnModuleChangesPublicationSelected != null)
                                        this.OnModuleChangesPublicationSelected(module, this.commitMessage);
                                    this.modulesToggleGroup.Select(false);
                                }
                            }     
                            if (!module.IsDirty)
                            {
                                if (GUILayout.Button("Publish " + majorVersion.ToString(), GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                                {
                                    if (string.IsNullOrEmpty(this.commitMessage) || this.commitMessage == ModulePublicationTab.DefaultCommitMessage)
                                    {
                                        if (this.OnMessageTriggered != null)
                                            this.OnMessageTriggered(new Message(errorMessage, MessageType.Info));
                                    }
                                    else
                                    {
                                        if (this.OnModuleVersionPublicationSelected != null)
                                            this.OnModuleVersionPublicationSelected(module, this.commitMessage, majorVersion);
                                        this.modulesToggleGroup.Select(false);
                                    }
                                }
                                if (GUILayout.Button("Publish " + minorVersion.ToString(), GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                                {
                                    if (string.IsNullOrEmpty(this.commitMessage) || this.commitMessage == ModulePublicationTab.DefaultCommitMessage)
                                    {
                                        if (this.OnMessageTriggered != null)
                                            this.OnMessageTriggered(new Message(errorMessage, MessageType.Info));
                                    }
                                    else
                                    {
                                        if (this.OnModuleVersionPublicationSelected != null)
                                            this.OnModuleVersionPublicationSelected(module, this.commitMessage, minorVersion);
                                        this.modulesToggleGroup.Select(false);
                                    }
                                }
                                if (GUILayout.Button("Publish " + patchVersion.ToString(), GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                                {
                                    if (string.IsNullOrEmpty(this.commitMessage) || this.commitMessage == ModulePublicationTab.DefaultCommitMessage)
                                    {
                                        if (this.OnMessageTriggered != null)
                                            this.OnMessageTriggered(new Message(errorMessage, MessageType.Info));
                                    }
                                    else
                                    {
                                        if (this.OnModuleVersionPublicationSelected != null)
                                            this.OnModuleVersionPublicationSelected(module, this.commitMessage, patchVersion);
                                        this.modulesToggleGroup.Select(false);
                                    }
                                }
                            }
                        }
                        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                        }
                    }
                    GUILayout.Space(15f);
                }
            }
        }

        public void Reset()
        {
            this.commitMessage = ModulePublicationTab.DefaultCommitMessage;
            this.modulesToggleGroup.Select(false);
        }

        IEnumerable<ASelector<IModule>> CreateSelectors(IEnumerable<IModule> projectModules, IEnumerable<Tuple<IRepositoryManager,IModule>> modules)
        {
            List<ASelector<IModule>> selectors = new List<ASelector<IModule>>();
            SimpleModuleSelector selector;
            Tuple<IRepositoryManager,IModule> tupleModule;
            IEnumerable<IModule> validProjectModules = projectModules.Where((IModule module) => !module.CurrentBranchVersion.IsVersion);
            foreach (IModule projectModule in validProjectModules)
            {
                tupleModule = modules.First((Tuple<IRepositoryManager, IModule> tupleModuleSearched) => tupleModuleSearched.Item2.UUID == projectModule.UUID);
                selector = new SimpleModuleSelector(new Tuple<IRepositoryManager, IModule>(tupleModule.Item1, projectModule), styles);
                selector.ModuleDetailsDisplay = new ModuleRepositoryStatus(projectModule, this.styles);
                selectors.Add(selector); 
            }
            return selectors;
        }

        void OnProjectModulesUpdated(IEnumerable<IModule> projectModules)
        { 
            this.modulesToggleGroup.Selectors = this.CreateSelectors(projectModules, this.moduniModel.Modules);
        }
    }
}

