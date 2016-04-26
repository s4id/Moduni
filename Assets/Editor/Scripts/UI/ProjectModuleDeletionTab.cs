//
//  ProjectModuleDeletionTab.cs
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
using Moduni.UI;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace Moduni
{
    public class ProjectModuleDeletionTab : ITab, IMessageTrigger
    {
        private ModulesImportationSelector modulesSelector;
        private IModuniModel moduniModel;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ProjectModuleDeletionTab(IModuniModel moduniModel, GUIStyles styles)
        {
            this.styles = styles;
            this.moduniModel = moduniModel;
            this.modulesSelector = new ModulesImportationSelector(this.CreateSelectors(this.moduniModel.ProjectModules, this.moduniModel.Modules));
            this.modulesSelector.DisplayDependenciesResolvingOption = false;
            this.moduniModel.OnProjectModulesUpdated += this.OnProjectModulesUpdated;
        }

        public delegate void OnModulesSelectedHandler(IEnumerable<IModule> modulesSelected);

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModulesSelectedHandler OnModulesSelected;

        public string Name
        {
            get
            {
                return "Deletion from project";
            }
        }

        public void Display()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (!this.moduniModel.RepositoriesManagers.Any())
            {
                EditorGUILayout.LabelField("No repositories found. Please, create a repository and modules before you can delete anything.", EditorStyles.boldLabel, null);
            }
            else if (!this.moduniModel.ProjectModules.Any())
            {
                EditorGUILayout.LabelField("No modules found in your project. Please, import modules before you can delete anything.", EditorStyles.boldLabel, null);
            }
            else
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Click on the name of each module you want to import and then click on 'Delete modules'."), this.styles.BoldLabelStyle, GUILayout.MinWidth(500f));
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                {
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
                    {
                        this.scrollPosition = scrollView.scrollPosition;
                        this.modulesSelector.Display();
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
                        if (GUILayout.Button("Delete modules from project", GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                        {
                            IEnumerable<IModule> modules = this.modulesSelector.ObjectSelected;
                            if (modules.FirstOrDefault() != null)
                            {
                                if (this.OnModulesSelected != null)
                                    this.OnModulesSelected(modules);
                                if (this.OnMessageTriggered != null)
                                    this.OnMessageTriggered(new Message("Modules successfully deleted from the project !", MessageType.Info));
                                this.Reset();
                            }
                            else
                            {
                                if (this.OnMessageTriggered != null)
                                    this.OnMessageTriggered(new Message("You haven't selected any modules to be deleted !", MessageType.Warning));
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
            this.modulesSelector.Select(false);
        }

        IEnumerable<ASelector<IModule>> CreateSelectors(IEnumerable<IModule> projectModules, IEnumerable<Tuple<IRepositoryManager,IModule>> modules)
        {
            List<ASelector<IModule>> selectors = new List<ASelector<IModule>>();
            SimpleModuleSelector selector;
            Tuple<IRepositoryManager,IModule> tupleModule;
            foreach (IModule projectModule in projectModules)
            {
                tupleModule = modules.First((Tuple<IRepositoryManager, IModule> tupleModuleSearched) => tupleModuleSearched.Item2.UUID == projectModule.UUID);
                selector = new SimpleModuleSelector(new Tuple<IRepositoryManager, IModule>(tupleModule.Item1, projectModule), styles);
                selector.ModuleDetailsDisplay = new ModuleDetails(projectModule, modules, this.styles);
                selectors.Add(selector); 
            }
            return selectors;
        }

        void OnProjectModulesUpdated(IEnumerable<IModule> projectModules)
        { 
            this.modulesSelector.Selectors = this.CreateSelectors(projectModules, this.moduniModel.Modules);
        }
    }
}

