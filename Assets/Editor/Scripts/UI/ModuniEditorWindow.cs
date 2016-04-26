//
//  ModuniEditorWindow.cs
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
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Callbacks;

namespace Moduni.UI
{
    public class ModuniEditorWindow : EditorWindow
    {
        private ITab activeTab;
        private MessageBoard messageBoard;
        private ModuniModel moduniModel;
        private int selectedToolbarButton;
        private GUIStyles styles;
        private ITab[] tabs;
        private string[] tabsNames;
        private GUIStyle toolbarStyle;

        public ModuniEditorWindow()
        {
            this.moduniModel = new ModuniModel();
            this.moduniModel.OnMessageTriggered += this.OnMessageTriggered;

            this.styles = new GUIStyles();

            ModuleCreationTab moduleCreationTab = new ModuleCreationTab(this.moduniModel, this.styles);
            moduleCreationTab.OnMessageTriggered += this.OnMessageTriggered;
            moduleCreationTab.OnModuleCreationCompleted += this.OnModuleCreationCompleted;
            ModuleModificationTab moduleModificationTab = new ModuleModificationTab(this.moduniModel, this.styles);
            moduleModificationTab.OnMessageTriggered += this.OnMessageTriggered;
            moduleModificationTab.OnModuleModificationCompleted += this.OnModuleModificationCompleted;
            ModuleImportationTab moduleImportationTab = new ModuleImportationTab(this.moduniModel, this.styles);
            moduleImportationTab.OnMessageTriggered += this.OnMessageTriggered;
            moduleImportationTab.OnModulesSelected += this.OnModulesSelectedForImportation;
            ModulePublicationTab modulePublicationTab = new ModulePublicationTab(this.moduniModel, this.styles);
            modulePublicationTab.OnMessageTriggered += this.OnMessageTriggered;
            modulePublicationTab.OnModuleChangesPublicationSelected += this.OnModuleSelectedForChangesPublication;
            modulePublicationTab.OnModuleVersionPublicationSelected += this.OnModuleSelectedForVersionPublication;
            ProjectModuleDeletionTab projectModuleDeletionTab = new ProjectModuleDeletionTab(this.moduniModel, this.styles);
            projectModuleDeletionTab.OnMessageTriggered += this.OnMessageTriggered;
            projectModuleDeletionTab.OnModulesSelected += this.OnModulesSelectedForDeletion;
            ConfigurationTab configurationTab = new ConfigurationTab(this.moduniModel.DeveloperSettings, this.moduniModel.RepositoriesManagersSettings, this.styles);
            configurationTab.OnConfigurationSaved += this.OnConfigurationSaved;
            configurationTab.OnMessageTriggered += this.OnMessageTriggered;
            this.tabs = new ITab[]
            {
                moduleCreationTab,
                moduleModificationTab,
                moduleImportationTab,
                modulePublicationTab,
                projectModuleDeletionTab,
                configurationTab
            };
            this.tabsNames = new string[this.tabs.Length];
            for (int i = 0; i < this.tabs.Length; i++)
                this.tabsNames[i] = this.tabs[i].Name;
            this.activeTab = this.tabs[0];
            this.selectedToolbarButton = 0;

            this.messageBoard = new MessageBoard();
            EditorApplication.update += () => this.Repaint();
        }

        [MenuItem("Window/Moduni")]
        public static void ShowWindow()
        {
            ModuniEditorWindow moduniEditorWindow = EditorWindow.GetWindow<ModuniEditorWindow>("Moduni", true, typeof(SceneView));
            moduniEditorWindow.minSize = new Vector2(640, 480);
        }

        [DidReloadScripts]
        static void DidReloadScripts()
        {
            ModuniEditorWindow moduniEditorWindow = EditorWindow.GetWindow<ModuniEditorWindow>("Moduni", true);
            moduniEditorWindow.OnScriptsReloaded();
        }

        public void OnScriptsReloaded()
        {
            this.activeTab.Reset();
            this.activeTab = this.tabs[this.selectedToolbarButton]; 
        }

        void OnGUI()
        {
            int previousSelectedToolbarButton = this.selectedToolbarButton;
            this.selectedToolbarButton = GUILayout.Toolbar(this.selectedToolbarButton, this.tabsNames, this.styles.ToolbarStyle);
            if (this.selectedToolbarButton != previousSelectedToolbarButton)
            {
                this.activeTab.Reset();
                this.activeTab = this.tabs[this.selectedToolbarButton];
            }

            this.activeTab.Display();
            this.messageBoard.Display();
        }

        void OnConfigurationSaved(DeveloperSettings developerSettings, IList<ARepositoryManagerSettings> repositoriesManagersSettings, bool isProjectOnlySettings)
        {
            this.moduniModel.UpdateSettings(developerSettings, repositoriesManagersSettings, isProjectOnlySettings);
        }

        void OnMessageTriggered(Message message)
        {
            this.messageBoard.AddMessage(message);
        }

        void OnModuleCreationCompleted(IRepositoryManager repositoryManager, ModuleState moduleState, Moduni.BranchVersion version)
        {
            this.moduniModel.CreateModule(repositoryManager, moduleState, version);
        }

        void OnModuleModificationCompleted(IModule module, ModuleState moduleState)
        {
            this.moduniModel.ModifyModule(module, moduleState);
        }

        void OnModulesSelectedForImportation(IEnumerable<IModule> modulesSelectedForImportation)
        {
            this.moduniModel.ImportModules(modulesSelectedForImportation);
        }

        void OnModuleSelectedForChangesPublication(IModule modulesSelectedForChangesPublication, string commitMessage)
        {
            this.moduniModel.PublishChanges(modulesSelectedForChangesPublication, commitMessage);
        }

        void OnModuleSelectedForVersionPublication(IModule modulesSelectedForVersionPublication, string versionMessage, BranchVersion newVersion)
        {
            this.moduniModel.PublishVersion(modulesSelectedForVersionPublication, versionMessage, newVersion);
        }

        void OnModulesSelectedForDeletion(IEnumerable<IModule> modulesSelectedForDeletion)
        {
            this.moduniModel.DeleteModules(modulesSelectedForDeletion);
        }
    }
}