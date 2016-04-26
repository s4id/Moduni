//
//  DependenciesEditor.cs
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
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Moduni.UI
{
    public class DependenciesEditor : AEditor<IList<Dependency>>
    {
        private List<Tuple<IRepositoryManager,IModule>> modulesSelectable;
        private List<Tuple<IRepositoryManager,IModule>> modulesSelected;
        private string[] modulesLabels;
        private int indexModuleSelected;
        private Vector2 scrollPosition;
        private List<string[]> versionsLabels;
        private List<int> versionsSelected;

        public List<Tuple<IRepositoryManager, IModule>> ModulesSelectable
        {
            set
            {
                this.modulesSelectable = value;
                this.modulesSelected.Clear();
                this.versionsLabels.Clear();
                this.versionsSelected.Clear();
                this.ResetModuleSelection();

            }
        }

        public DependenciesEditor(IList<Dependency> dependencies, List<Tuple<IRepositoryManager,IModule>> modulesSelectable)
        {
            this.objectEdited = dependencies;
            this.modulesSelected = new List<Tuple<IRepositoryManager, IModule>>();
            this.versionsLabels = new List<string[]>();
            this.versionsSelected = new List<int>();
            this.ModulesSelectable = modulesSelectable;
        }

        public override void Display()
        {
            int previousVersionSelected;
            Color previousBackgroundColor;
            using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
            {
                this.scrollPosition = scrollView.scrollPosition;

                if (this.modulesSelectable.Count > 0)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        this.indexModuleSelected = EditorGUILayout.Popup(this.indexModuleSelected, this.modulesLabels, GUI.skin.button, GUILayout.MaxWidth(200f));
                        if (GUILayout.Button("Add new dependency"))
                        {
                            Tuple<IRepositoryManager,IModule> moduleSelected = this.modulesSelectable[this.indexModuleSelected];
                            this.modulesSelected.Add(moduleSelected);
                            this.modulesSelectable.RemoveAt(this.indexModuleSelected);
                            this.versionsSelected.Add(0);
                            this.versionsLabels.Add(moduleSelected.Item2.Versions.Select<BranchVersion,string>((BranchVersion version) => version.ToString()).ToArray());
                            Dependency dependency = new Dependency();
                            this.objectEdited.Add(dependency);
                            this.UpdateDependency(this.objectEdited.Count - 1);

                            this.ResetModuleSelection();
                        }
                    }
                }

                for (int i = this.objectEdited.Count - 1; i >= 0; --i)
                {
                    previousBackgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = this.modulesSelected[i].Item1.Color;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope(GUI.skin.textArea))
                        {
                            GUI.backgroundColor = previousBackgroundColor;
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField(string.Format("({0}) {1}", this.modulesSelected[i].Item1.Name, this.modulesSelected[i].Item2.Name), EditorStyles.boldLabel);
                                EditorGUILayout.PrefixLabel(new GUIContent("Version: "), GUI.skin.textField, GUI.skin.label);
                                previousVersionSelected = this.versionsSelected[i];
                                this.versionsSelected[i] = EditorGUILayout.Popup(this.versionsSelected[i], this.versionsLabels[i], EditorStyles.toolbarPopup, GUILayout.MaxWidth(100f), GUILayout.ExpandHeight(false));
                                if (this.versionsSelected[i] != previousVersionSelected)
                                {
                                    this.UpdateDependency(i);
                                }
                            }
                            GUILayout.FlexibleSpace();
                        }
                        if (GUILayout.Button("-"))
                        {
                            Tuple<IRepositoryManager,IModule> moduleSelected = this.modulesSelected[i];
                            this.modulesSelectable.Add(moduleSelected);
                            this.modulesSelected.RemoveAt(i);
                            this.objectEdited.RemoveAt(i);
                            this.versionsLabels.RemoveAt(i);
                            this.versionsSelected.RemoveAt(i);

                            this.ResetModuleSelection();
                        }
                    }
                }
            }
        }

        void ResetModuleSelection()
        {
            this.indexModuleSelected = 0;
            this.modulesLabels = this.modulesSelectable.Select<Tuple<IRepositoryManager, IModule>,string>((Tuple<IRepositoryManager, IModule> module) => string.Format("({0}) {1}", module.Item1.Name, module.Item2.Name)).ToArray();
        }

        void UpdateDependency(int index)
        {
            this.objectEdited[index].UUIDModuleRequired = this.modulesSelected[index].Item2.UUID;
            this.objectEdited[index].MinimumVersion = new Moduni.BranchVersion(this.versionsLabels[index][this.versionsSelected[index]]);
        }
    }
}

