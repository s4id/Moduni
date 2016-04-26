//
//  ModuleCreationTab.cs
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moduni.UI
{
    public class ModuleCreationTab : ITab, IMessageTrigger
    {
        private IUIElement currentState;
        private ModuleStateEditor moduleEditorState;
        private IModuniModel moduniModel;
        private IRepositoryManager repositoryManagerSelected;
        private RepositoryManagerSelector repositoryManagerSelectorState;
        private GUIStyles styles;

        public ModuleCreationTab(IModuniModel moduniModel, GUIStyles styles)
        {
            this.styles = styles;
            this.moduniModel = moduniModel;

            this.repositoryManagerSelectorState = new RepositoryManagerSelector(this.moduniModel.RepositoriesManagers);
            this.repositoryManagerSelectorState.OnSelected += this.OnRepositoryManagerSelected;

            this.moduleEditorState = new ModuleStateEditor(moduniModel.ModuleFactory.CreateModuleState(), moduniModel.Modules, "Create module", this.styles);
            this.moduleEditorState.OnMessageTriggered += delegate(Message message)
            {
                if (this.OnMessageTriggered != null)
                    this.OnMessageTriggered(message);
            };
            this.moduleEditorState.OnEditionCompleted += this.OnModuleEditionCompleted;
            this.moduniModel.OnModulesUpdated += this.OnModulesUpdated;
            this.moduniModel.OnRepositoriesManagersUpdated += this.OnRepositoriesManagersUpdated;

            this.currentState = this.repositoryManagerSelectorState;
        }

        public delegate void OnModuleCreationCompletedHandler(IRepositoryManager repositoryManager,ModuleState moduleState,Moduni.BranchVersion version);

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModuleCreationCompletedHandler OnModuleCreationCompleted;

        public string Name
        {
            get
            {
                return "Creation";
            }
        }

        public void Display()
        {
            EditorGUILayout.Space();
            if (!this.moduniModel.RepositoriesManagers.Any())
                EditorGUILayout.LabelField(new GUIContent("No repositories managers found. Please create one before you can proceed."), this.styles.BoldLabelStyle, null);
            else
                EditorGUILayout.LabelField(new GUIContent("Select the repository manager for the module."), this.styles.BoldLabelStyle, null);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            this.currentState.Display();
        }

        public void Reset()
        {
            this.currentState = this.repositoryManagerSelectorState;
            this.moduleEditorState.ObjectEdited = this.moduniModel.ModuleFactory.CreateModuleState();
        }

        void OnModuleEditionCompleted(ModuleState moduleState)
        {
            this.currentState = this.repositoryManagerSelectorState;
            if (this.OnModuleCreationCompleted != null)
            {
                this.OnModuleCreationCompleted(this.repositoryManagerSelected, moduleState, this.moduleEditorState.Version);
                this.moduleEditorState.ObjectEdited = this.moduniModel.ModuleFactory.CreateModuleState();
            }
            if (this.OnMessageTriggered != null)
            {
                this.OnMessageTriggered(new Message("Module created with success.", MessageType.Info));
            }
        }

        void OnModulesUpdated(IEnumerable<Tuple<IRepositoryManager,IModule>> modules)
        {
            this.moduleEditorState.Modules = modules;
        }

        void OnRepositoriesManagersUpdated(IEnumerable<IRepositoryManager> repositoriesManagers)
        {
            this.repositoryManagerSelectorState.RepositoriesManagers = repositoriesManagers;
        }

        void OnRepositoryManagerSelected(IRepositoryManager repositoryManager)
        {
            this.repositoryManagerSelected = repositoryManager;
            this.currentState = this.moduleEditorState;
        }
    }
}

