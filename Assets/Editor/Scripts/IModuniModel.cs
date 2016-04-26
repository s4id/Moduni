//
//  IModuniModel.cs
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

namespace Moduni
{
    using System;
    using System.Collections.Generic;

    public delegate void OnModulesUpdatedHandler(IEnumerable<Tuple<IRepositoryManager, IModule>> modules);
    public delegate void OnProjectModulesUpdatedHandler(IEnumerable<IModule> projectModules);
    public delegate void OnRepositoriesManagersUpdatedHandler(IEnumerable<IRepositoryManager> repositories);
    public delegate void OnRepositoriesManagersSettingsUpdatedHandler(IEnumerable<ARepositoryManagerSettings> repositoriesManagersSettings);

    public interface IModuniModel : IMessageTrigger
    {
        event OnModulesUpdatedHandler OnModulesUpdated;
        event OnProjectModulesUpdatedHandler OnProjectModulesUpdated;
        event OnRepositoriesManagersUpdatedHandler OnRepositoriesManagersUpdated;
        event OnRepositoriesManagersSettingsUpdatedHandler OnRepositoriesManagersSettingsUpdated;

        DeveloperSettings DeveloperSettings { get; }

        ModuleFactory ModuleFactory { get; }

        IEnumerable<Tuple<IRepositoryManager, IModule>> Modules { get; }

        IEnumerable<IModule> ProjectModules { get; }

        IEnumerable<IRepositoryManager> RepositoriesManagers { get; }

        IEnumerable<ARepositoryManagerSettings> RepositoriesManagersSettings { get; }

        void CreateModule(IRepositoryManager repositoryManager, ModuleState moduleState, Moduni.BranchVersion version);

        void ModifyModule(IModule module, ModuleState moduleState);

        void DeleteModules(IEnumerable<IModule> modulesToDelete);

        void ImportModules(IEnumerable<IModule> modulesToImport);

        void PublishChanges(IModule modulePublished, string commitMessage);

        void PublishVersion(IModule modulePublished, string versionMessage, BranchVersion newVersion);
    }
}

