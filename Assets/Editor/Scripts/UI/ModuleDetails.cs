//
//  ModuleDetails.cs
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
using System.Linq;
using System.Globalization;

namespace Moduni.UI
{
    public class ModuleDetails : IUIElement
    {
        private bool foldout;
        private IModule module;
        private IEnumerable<Tuple<IRepositoryManager,IModule>> otherModules;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ModuleDetails(IModule module, IEnumerable<Tuple<IRepositoryManager,IModule>> otherModules, GUIStyles styles)
        {
            this.module = module;
            this.otherModules = otherModules;
            this.styles = styles;
        }

        public void Display()
        {
            Tuple<IRepositoryManager,IModule> tupleModuleDependency;
            CommitSignature creator = module.Creator;
            CommitSignature versionAuthor = module.VersionAuthor;

            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false)))
            {
                GUILayout.Space(125f);
                using (new EditorGUILayout.VerticalScope(GUI.skin.textArea, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                {
                    this.foldout = EditorGUILayout.Foldout(this.foldout, "Details");
                    if (this.foldout)
                    {
                        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition, GUILayout.MaxHeight(400f), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false)))
                        {
                            this.scrollPosition = scrollView.scrollPosition;

                            GUILayout.Space(5f);
                            EditorGUILayout.LabelField("Description", this.styles.BoldLabelStyle, GUILayout.ExpandWidth(false));
                            GUILayout.Space(5f);
                            EditorGUILayout.LabelField(module.Description, GUI.skin.textArea, GUILayout.ExpandWidth(true));
                            GUILayout.Space(5f);
                            EditorGUILayout.LabelField("Created by " + creator.name + " on " + creator.when.LocalDateTime.ToString(CultureInfo.InvariantCulture), EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MinWidth(400f));
                            GUILayout.Space(5f);
                            EditorGUILayout.LabelField("Version created by " + versionAuthor.name + " on " + versionAuthor.when.LocalDateTime.ToString(CultureInfo.InvariantCulture), EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MinWidth(400f));
                            GUILayout.Space(5f);
                            if (module.Dependencies.Count > 0)
                                EditorGUILayout.LabelField("Dependencies", this.styles.BoldLabelStyle, GUILayout.ExpandWidth(false));
                            else
                                EditorGUILayout.LabelField("No dependencies", this.styles.BoldLabelStyle, GUILayout.ExpandWidth(false));
                            EditorGUI.indentLevel += 2;
                            foreach (Dependency dependency in module.Dependencies)
                            {
                                tupleModuleDependency = this.otherModules.FirstOrDefault((Tuple<IRepositoryManager, IModule> tuple) => tuple.Item2.UUID == dependency.UUIDModuleRequired);
                                EditorGUILayout.LabelField(string.Format("({0}) {1} [{2}]", tupleModuleDependency.Item1.Name, tupleModuleDependency.Item2.Name, dependency.MinimumVersion.ToString()), EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MinWidth(400f));
                            }
                            EditorGUI.indentLevel -= 2;
                        }
                    }
                }
            }
            GUILayout.Space(8f);
        }
    }
}

