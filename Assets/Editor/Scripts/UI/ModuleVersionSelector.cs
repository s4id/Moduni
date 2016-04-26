//
//  ModuleVersionSelector.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Moduni.UI
{
    public class ModuleVersionSelector : ASelector<IModule>
    {
        private bool isDisabled;
        private BranchVersion maxVersion;
        private IUIElement moduleDetailsDisplay;
        private Tuple<IRepositoryManager,IModule> moduleSelectable;
        private IModule projectModule;
        private ModuleRepositoryStatus repositoryStatus;
        private GUIStyles styles;
        private string[] versionsAvailable;
        private int versionSelected;

        public ModuleVersionSelector(Tuple<IRepositoryManager,IModule> moduleSelectable, IModule projectModule, GUIStyles styles)
        {
            this.moduleSelectable = moduleSelectable;
            this.projectModule = projectModule;
            this.styles = styles;

            if (this.projectModule != null)
                this.repositoryStatus = new ModuleRepositoryStatus(projectModule, styles);

            this.maxVersion = this.moduleSelectable.Item2.Versions.Where((BranchVersion branchVersion) => branchVersion.IsVersion).Max();
            if (projectModule != null)
                this.versionSelected = this.moduleSelectable.Item2.Versions.FindIndex((BranchVersion branchVersion) => branchVersion.ToString() == projectModule.CurrentBranchVersion.ToString());
            else
                this.versionSelected = this.moduleSelectable.Item2.Versions.FindIndex((BranchVersion branchVersion) => branchVersion.ToString() == "master"); 
            this.versionsAvailable = this.moduleSelectable.Item2.Versions.Select<BranchVersion,string>((BranchVersion branchVersion) => branchVersion.ToString()).ToArray();
        }

        public override bool IsDisabled
        {
            set
            {
                this.isDisabled = value;
            }
        }

        public IUIElement ModuleDetailsDisplay
        {
            set
            {
                this.moduleDetailsDisplay = value;
            }
        }

        public override void Display()
        {
            IRepositoryManager repositoryManagerModuleSelectable = this.moduleSelectable.Item1;
            IModule module = this.moduleSelectable.Item2;
            Color previousBackgroundColor;
            int previousVersionSelected;

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
                        else
                        {
                            if (this.projectModule != null)
                            {
                                if (this.projectModule.CurrentBranchVersion >= this.maxVersion)
                                {
                                    GUI.backgroundColor = Colors.LightGreen;
                                }
                                else
                                {
                                    GUI.backgroundColor = Colors.LightRed;
                                }
                            }
                        }

                        if (this.projectModule != null)
                        {
                            if (GUILayout.Button(string.Format("({0}) {1} [{2}]", repositoryManagerModuleSelectable.Name, module.Name, this.projectModule.CurrentBranchVersion.ToString()), 
                                    this.styles.ButtonStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(50f)))
                            {
                                this.ObjectSelected = (this.objectSelected == null) ? module : null;
                            }
                        }
                        else
                        {
                            if (GUILayout.Button(string.Format("({0}) {1}", repositoryManagerModuleSelectable.Name, module.Name), 
                                    this.styles.ButtonStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(50f), GUILayout.MinWidth(250f)))
                            {
                                this.ObjectSelected = (this.objectSelected == null) ? module : null;
                            }
                        }
                        GUI.backgroundColor = previousBackgroundColor;
                    }
                    GUILayout.Space(20f);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Space(4f);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Version", EditorStyles.label, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(60f));
                            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(false), GUILayout.MaxWidth(100f)))
                            {
                                previousVersionSelected = this.versionSelected;
                                this.versionSelected = EditorGUILayout.Popup(this.versionSelected, this.versionsAvailable, EditorStyles.toolbarPopup, GUILayout.MaxWidth(100f), GUILayout.ExpandHeight(false));
                                if (this.versionSelected != previousVersionSelected)
                                {
                                    module.CurrentBranchVersion = new BranchVersion(this.versionsAvailable[this.versionSelected]);
                                }
                            }
                        }
                        GUILayout.Space(10f);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            previousBackgroundColor = GUI.backgroundColor;
                            GUI.backgroundColor = Colors.Orange;
                            EditorGUILayout.LabelField("TRL " + module.TRL.ToString(), EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(65f));
                            GUI.backgroundColor = previousBackgroundColor;

                            GUILayout.Space(10f);

                            this.DisplayTags(module.Tags);
                        }
                    }
                }
                // DETAILS OF THE MODULE
                this.moduleDetailsDisplay.Display();
                if (this.repositoryStatus != null)
                    this.repositoryStatus.Display();
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

        void DisplayTags(IEnumerable<Tag> tags)
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
    }
}

