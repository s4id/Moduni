//
//  SimpleModuleSelector.cs
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
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Moduni.UI
{
    public class SimpleModuleSelector : ASelector<IModule>
    {
        private bool isDisabled;
        private IUIElement moduleDetailsDisplay;
        private Tuple<IRepositoryManager,IModule> moduleSelectable;
        private GUIStyles styles;

        public IUIElement ModuleDetailsDisplay
        {
            set
            {
                this.moduleDetailsDisplay = value;
            }
        }

        public SimpleModuleSelector(Tuple<IRepositoryManager,IModule> moduleSelectable, GUIStyles styles)
        {
            this.moduleSelectable = moduleSelectable;
            this.styles = styles;
        }

        public void DisplayTags(IEnumerable<Tag> tags)
        {
            Color previousBackgroundColor;

            foreach (Tag tag in tags)
            {
                previousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = tag.color;
                EditorGUILayout.LabelField(tag.name, EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150f));
                GUI.backgroundColor = previousBackgroundColor;
            }
        }

        #region implemented abstract members of ASelector

        public override void Display()
        {
            IRepositoryManager repositoryManagerModuleSelectable = this.moduleSelectable.Item1;
            IModule module = this.moduleSelectable.Item2;
            Color previousBackgroundColor;

            previousBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = repositoryManagerModuleSelectable.Color;
            using (new EditorGUILayout.VerticalScope(GUI.skin.textArea))
            {
                GUI.backgroundColor = previousBackgroundColor;
                GUILayout.Space(8f);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(110f);
                    using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(175f)))
                    {
                        previousBackgroundColor = GUI.backgroundColor;
                        if (this.objectSelected != null)
                        {
                            GUI.backgroundColor = Colors.SkyBlue;
                        }
                        if (GUILayout.Button(string.Format("({0}) {1} [{2}]", repositoryManagerModuleSelectable.Name, module.Name, module.CurrentBranchVersion.ToString()), 
                                this.styles.ButtonStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(50f)))
                        {
                            this.ObjectSelected = (this.objectSelected == null) ? module : null;
                        }
                        GUI.backgroundColor = previousBackgroundColor;
                    }
                    GUILayout.Space(20f);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Space(4f);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            previousBackgroundColor = GUI.backgroundColor;
                            GUI.backgroundColor = Colors.Orange;
                            EditorGUILayout.LabelField("TRL " + module.TRL.ToString(), EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(65f));
                            GUI.backgroundColor = previousBackgroundColor;
                        }
                        GUILayout.Space(10f);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            this.DisplayTags(module.Tags);
                        }
                    }
                }
                // DETAILS OF THE MODULE
                this.moduleDetailsDisplay.Display();
                EditorGUILayout.Separator();
            }
        }

        public override void Select(bool selected)
        {
            if (selected)
            {
                this.ObjectSelected = this.moduleSelectable.Item2;
            }
            else
            {
                this.ObjectSelected = null;
            }
        }

        public override bool IsDisabled
        {
            set
            {
                this.isDisabled = value;
            }
        }

        #endregion
    }
}

