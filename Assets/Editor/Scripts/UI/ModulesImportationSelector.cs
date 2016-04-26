//
//  ModulesImportationSelector.cs
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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class ModulesImportationSelector : ASelector<IEnumerable<IModule>>
    {
        private bool displayDependenciesResolvingOption;
        private bool isResolvingDependencies;
        private List<IModule> modulesSelected;
        private IEnumerable<ASelector<IModule>> selectors;
        private GUIStyles styles;

        public ModulesImportationSelector(IEnumerable<ASelector<IModule>> selectors)
        {
            this.modulesSelected = new List<IModule>();
            this.objectSelected = this.modulesSelected;
            this.Selectors = selectors;
            this.displayDependenciesResolvingOption = true;
        }

        public bool DisplayDependenciesResolvingOption
        {
            set
            {
                this.displayDependenciesResolvingOption = value;
            }
        }

        public override bool IsDisabled
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ASelector<IModule>> Selectors
        {
            set
            {
                if (this.selectors != null)
                {
                    foreach (ASelector<IModule> selector in this.selectors)
                    {
                        selector.OnSelected -= this.OnModuleSelected;
                        selector.OnDeselected -= this.OnModuleDeselected;
                    }
                }
                this.selectors = value;
                foreach (ASelector<IModule> selector in this.selectors)
                {
                    selector.OnSelected += this.OnModuleSelected;
                    selector.OnDeselected -= this.OnModuleDeselected;
                }
            }
        }

        public override void Display()
        {
            Color previousBackgroundColor;

            foreach (ASelector<IModule> selector in this.selectors)
            {
                selector.Display();
            }

            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
            {
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
                // Some options to configure the selection
                if (GUILayout.Button("Select all", GUILayout.MinWidth(145f), GUILayout.MinHeight(45f), GUILayout.Width(145f)))
                {
                    this.Select(true);
                }
                if (GUILayout.Button("Select none", GUILayout.MinWidth(145f), GUILayout.MinHeight(45f), GUILayout.Width(145f)))
                {
                    this.Select(false);
                }

                if (this.displayDependenciesResolvingOption)
                {
                    previousBackgroundColor = GUI.backgroundColor;
                    if (this.isResolvingDependencies)
                        GUI.backgroundColor = Colors.LightGreen;
                    else
                        GUI.backgroundColor = Colors.LightRed;
                    if (GUILayout.Button("Resolve dependencies", GUILayout.MinWidth(145f), GUILayout.MinHeight(45f), GUILayout.Width(145f)))
                    {
                        this.isResolvingDependencies = !this.isResolvingDependencies;
                    }
                    GUI.backgroundColor = previousBackgroundColor;
                }
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();
            }
        }

        public override void Select(bool selected)
        {
            this.modulesSelected.Clear();
            foreach (ASelector<IModule> selector in this.selectors)
            {
                selector.Select(selected);
            }
        }

        void OnModuleDeselected(IModule module)
        {
            this.modulesSelected.Remove(module);
            this.ObjectSelected = this.modulesSelected;
        }

        void OnModuleSelected(IModule module)
        {
            if (module != null)
            {
                this.modulesSelected.Add(module);
                this.ObjectSelected = this.modulesSelected;
            }
        }
    }
}

