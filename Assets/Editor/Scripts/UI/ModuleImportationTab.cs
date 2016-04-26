//
//  ModuleImportationTab.cs
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
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Moduni.UI
{
    public class ModuleImportationTab : ITab, IMessageTrigger
    {
        private ModulesImportationSelector modulesSelector;
        private IModuniModel moduniModel;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ModuleImportationTab(IModuniModel moduniModel, GUIStyles styles)
        {
            this.styles = styles;
            this.moduniModel = moduniModel;
            this.moduniModel.OnModulesUpdated += this.OnModulesUpdated;
            this.moduniModel.OnProjectModulesUpdated += this.OnProjectModulesUpdated;
            this.modulesSelector = new ModulesImportationSelector(this.CreateSelectors(this.moduniModel.Modules, this.moduniModel.ProjectModules));
        }

        public delegate void OnModulesSelectedHandler(IEnumerable<IModule> modulesSelected);

        public event OnMessageTriggeredHandler OnMessageTriggered;
        public event OnModulesSelectedHandler OnModulesSelected;

        public string Name
        {
            get
            {
                return "Importation";
            }
        }

        public void Display()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (!this.moduniModel.RepositoriesManagers.Any())
            {
                EditorGUILayout.LabelField("No repositories found. Please, create a repository and modules before you can import anything.", EditorStyles.boldLabel, null);
            }
            else if (!this.moduniModel.Modules.Any())
            {
                EditorGUILayout.LabelField("No modules found in the repositories registered. Please, create modules before you can import anything.", EditorStyles.boldLabel, null);
            }
            else
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Click on the name of each module you want to import and then click on 'Import modules'."), this.styles.BoldLabelStyle, GUILayout.MinWidth(500f));
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
                        if (GUILayout.Button("Import modules", GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                        {
                            IEnumerable<IModule> modules = this.modulesSelector.ObjectSelected;
                            if (modules.FirstOrDefault() != null)
                            {
                                if (this.OnModulesSelected != null)
                                    this.OnModulesSelected(modules);
                                this.modulesSelector.Select(false);
                            }
                            else
                            {
                                if (this.OnMessageTriggered != null)
                                    this.OnMessageTriggered(new Message("You haven't selected any modules to be imported !", MessageType.Warning));
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
            this.modulesSelector.Selectors = this.CreateSelectors(this.moduniModel.Modules, this.moduniModel.ProjectModules);
        }

        IEnumerable<ASelector<IModule>> CreateSelectors(IEnumerable<Tuple<IRepositoryManager,IModule>> modules, IEnumerable<IModule> projectModules)
        {
            List<ASelector<IModule>> selectors = new List<ASelector<IModule>>();
            ModuleVersionSelector selector;
            foreach (Tuple<IRepositoryManager,IModule> tupleModule in modules)
            {
                selector = new ModuleVersionSelector(tupleModule, projectModules.FirstOrDefault((IModule module) => module.UUID == tupleModule.Item2.UUID), styles);
                selector.ModuleDetailsDisplay = new ModuleDetails(tupleModule.Item2, modules, this.styles);
                selectors.Add(selector); 
            }
            return selectors;
        }

        void OnModulesUpdated(IEnumerable<Tuple<IRepositoryManager,IModule>> modules)
        {
            this.modulesSelector.Selectors = this.CreateSelectors(modules, this.moduniModel.ProjectModules);
        }

        void OnProjectModulesUpdated(IEnumerable<IModule> projectModules)
        {
            this.modulesSelector.Selectors = this.CreateSelectors(this.moduniModel.Modules, projectModules);
        }
    }
}

