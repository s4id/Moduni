//
//  ModuleModificationTab.cs
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

using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Moduni.UI
{
    using UnityEngine;

    public class ModuleModificationTab : ITab, IMessageTrigger
    {
        private IUIElement currentState;
        private ModuleStateEditor moduleEditorState;
        private IModule moduleSelected;
        private ModulesToggleGroup modulesToggleGroupState;
        private IModuniModel moduniModel;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ModuleModificationTab(IModuniModel moduniModel, GUIStyles styles)
        {
            this.moduniModel = moduniModel;
            this.styles = styles;

            this.modulesToggleGroupState = new ModulesToggleGroup(this.CreateSelectors(moduniModel.ProjectModules, moduniModel.Modules));
            this.modulesToggleGroupState.OnSelected += this.OnModuleSelected;

            this.moduleEditorState = new ModuleStateEditor(this.moduniModel.ModuleFactory.CreateModuleState(), moduniModel.Modules, "Modify module", this.styles);
            this.moduleEditorState.IsCreation = false;
            this.moduleEditorState.IsVersionEditionEnabled = false;
            this.moduleEditorState.OnMessageTriggered += this.OnModuleEditorMessageTriggered;
            this.moduleEditorState.OnEditionCompleted += this.OnModuleEditionCompleted;

            this.moduniModel.OnProjectModulesUpdated += this.OnProjectModulesUpdated;

            this.currentState = this.modulesToggleGroupState;
        }

        public delegate void OnModuleModificationCompletedHandler(IModule module,ModuleState moduleState);

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModuleModificationCompletedHandler OnModuleModificationCompleted;

        public string Name
        {
            get
            {
                return "Modification";
            }
        }

        public void Display()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (!this.moduniModel.RepositoriesManagers.Any())
            {
                EditorGUILayout.LabelField("No repositories found. Please, create a repository and modules before you can modify them.", EditorStyles.boldLabel, null);
            }
            else if (!this.moduniModel.ProjectModules.Any())
            {
                EditorGUILayout.LabelField("No modules found in your project. Please, import or create modules before you can modify them.", EditorStyles.boldLabel, null);
            }
            else
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Select the name of the module you want to modify."), this.styles.BoldLabelStyle, GUILayout.MinWidth(500f));
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
                    {
                        this.scrollPosition = scrollView.scrollPosition;
                        this.currentState.Display();
                    }
                }
                EditorGUILayout.Separator();
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                        }
                        IModule module = this.modulesToggleGroupState.ObjectSelected;
                        if (module != null)
                        {
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
            this.modulesToggleGroupState.Select(false);
            this.currentState = this.modulesToggleGroupState;
        }

        IEnumerable<ASelector<IModule>> CreateSelectors(IEnumerable<IModule> projectModules, IEnumerable<Tuple<IRepositoryManager,IModule>> modules)
        {
            List<ASelector<IModule>> selectors = new List<ASelector<IModule>>();
            SimpleModuleSelector selector;
            Tuple<IRepositoryManager,IModule> tupleModule;
            IEnumerable<IModule> validProjectModules = projectModules.Where((IModule module) => module.CurrentBranchVersion.ToString() == "master");
            foreach (IModule projectModule in validProjectModules)
            {
                tupleModule = modules.First((Tuple<IRepositoryManager, IModule> tupleModuleSearched) => tupleModuleSearched.Item2.UUID == projectModule.UUID);
                selector = new SimpleModuleSelector(new Tuple<IRepositoryManager, IModule>(tupleModule.Item1, projectModule), styles);
                selector.ModuleDetailsDisplay = new ModuleDetails(projectModule, modules, this.styles);
                selectors.Add(selector); 
            }
            return selectors;
        }

        void OnModuleEditionCompleted(ModuleState moduleState)
        {
            if (this.OnModuleModificationCompleted != null)
                this.OnModuleModificationCompleted(this.moduleSelected, moduleState);

            this.modulesToggleGroupState.Select(false);

            this.currentState = this.modulesToggleGroupState;
        }

        void OnModuleEditorMessageTriggered(Message message)
        {
            if (this.OnMessageTriggered != null)
                this.OnMessageTriggered(message);
        }

        void OnModuleSelected(IModule module)
        {
            this.moduleSelected = module;

            this.moduleEditorState.ObjectEdited = module.CloneState();

            this.currentState = this.moduleEditorState;
        }

        void OnProjectModulesUpdated(IEnumerable<IModule> projectModules)
        { 
            this.modulesToggleGroupState.Selectors = this.CreateSelectors(projectModules, this.moduniModel.Modules);
        }
    }
}

