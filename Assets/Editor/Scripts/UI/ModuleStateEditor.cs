//
//  ModuleStateEditor.cs
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
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace Moduni.UI
{
    public class ModuleStateEditor : AEditor<ModuleState>, IMessageTrigger
    {
        private DependenciesEditor dependenciesEditors;
        private bool displayDependencies;
        private bool foldoutDependencies;
        private bool isCreation = true;
        private bool isVersionEditionEnabled = true;
        private IEnumerable<Tuple<IRepositoryManager,IModule>> modules;
        private Vector2 scrollPositionForm;
        private GUIStyles styles;
        private Tag tag;
        private string[] trlLabels;
        private string validationLabel;
        private Moduni.BranchVersion version;

        public ModuleStateEditor(ModuleState moduleStateToBeEdited, IEnumerable<Tuple<IRepositoryManager,IModule>> modules, string validationLabel, GUIStyles styles)
        {
            this.validationLabel = validationLabel;
            this.styles = styles;
            this.objectEdited = moduleStateToBeEdited;
            this.dependenciesEditors = new DependenciesEditor(this.objectEdited.Dependencies, modules.ToList());
            this.Modules = modules;

            this.version = new BranchVersion(1, 0, 0);
            this.tag = new Tag("New tag", Color.red);
            this.trlLabels = new string[]{ "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        }

        public event OnMessageTriggeredHandler OnMessageTriggered;

        public bool IsCreation
        {
            set
            {
                isCreation = value;
            }
        }

        public bool IsVersionEditionEnabled
        {
            set
            {
                this.isVersionEditionEnabled = value;
            }
        }

        public IEnumerable<Tuple<IRepositoryManager, IModule>> Modules
        {
            get
            {
                return this.modules;
            }
            set
            {
                this.modules = value;
                this.displayDependencies = this.modules.GetEnumerator().MoveNext();
                this.dependenciesEditors.ModulesSelectable = this.modules.ToList();
            }
        }

        public override ModuleState ObjectEdited
        {
            get
            {
                return this.objectEdited;
            }
            set
            {
                base.ObjectEdited = value;
                this.dependenciesEditors.ObjectEdited = this.objectEdited.Dependencies;
            }
        }

        public Moduni.BranchVersion Version
        {
            get
            {
                return version;
            }
        }

        public override void Display()
        {
            EditorGUILayout.Space();
            using (EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                }
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(GUILayout.MinWidth(600f), GUILayout.MaxWidth(1100f)))
                {
                    using (EditorGUILayout.VerticalScope verticalScope2 = new EditorGUILayout.VerticalScope(GUI.skin.textArea, GUILayout.ExpandHeight(true)))
                    {
                        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPositionForm))
                        {
                            this.scrollPositionForm = scrollView.scrollPosition;

                            using (EditorGUILayout.HorizontalScope horizontalScope2 = new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.Space(30f);
                                using (EditorGUILayout.VerticalScope verticalScope3 = new EditorGUILayout.VerticalScope())
                                {
                                    this.DisplayModuleForm();
                                }
                                GUILayout.Space(30f);
                            }
                        }
                    }
                    EditorGUILayout.Separator();

                    //this.GUICommitMessage();

                    using (EditorGUILayout.HorizontalScope horizontalScope2 = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(this.validationLabel, GUILayout.MinWidth(250f), GUILayout.MinHeight(50f), GUILayout.Width(250f)))
                        {
                            string resultValidation = this.objectEdited.Validate(isCreation);
                            if (string.IsNullOrEmpty(resultValidation))
                            {
                                this.TriggerEditionCompleted();
                            }
                            else
                            {
                                if (this.OnMessageTriggered != null)
                                    this.OnMessageTriggered(new Message(resultValidation, MessageType.Warning));
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                }
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        void DisplayModuleForm()
        {
            //NAME
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), this.styles.BoldLabelStyle, GUILayout.MaxWidth(100f));
                this.objectEdited.Name = EditorGUILayout.TextField(this.objectEdited.Name, GUI.skin.textField, GUILayout.MaxWidth(400f));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space();

            //PATH
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Path"), this.styles.BoldLabelStyle, GUILayout.MaxWidth(100f));
                if (GUILayout.Button("...", GUILayout.MaxWidth(40f)))
                {
                    string path = EditorUtility.SaveFolderPanel("Select the folder for the repository of your module", Application.dataPath, this.objectEdited.Name + ".git");
                    if (path.StartsWith(Application.dataPath))
                    {
                        this.objectEdited.Path = path;
                    }
                    else
                    {
                        if (this.OnMessageTriggered != null)
                            this.OnMessageTriggered(new Message("The repository should be inside the current project.", MessageType.Warning));
                    }
                }
                EditorGUILayout.LabelField(new GUIContent(this.objectEdited.Path), GUILayout.MaxWidth(800f));
            }
            EditorGUILayout.Space();

            //VERSION
            if (this.isVersionEditionEnabled)
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Version"), this.styles.BoldLabelStyle, GUILayout.MaxWidth(100f));
                    this.version.major = (uint)EditorGUILayout.IntField((int)version.major, GUILayout.MaxWidth(30f));
                    this.version.minor = (uint)EditorGUILayout.IntField((int)version.minor, GUILayout.MaxWidth(30f));
                    this.version.patch = (uint)EditorGUILayout.IntField((int)version.patch, GUILayout.MaxWidth(30f));
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Reset version", GUILayout.MaxWidth(100f)))
                    {
                        this.version = new BranchVersion(1, 0, 0);
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            // TRL
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Estimated TRL"), this.styles.BoldLabelStyle, null);
                this.objectEdited.TRL = GUILayout.Toolbar(this.objectEdited.TRL - 1, this.trlLabels, EditorStyles.toolbarButton, null) + 1;
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space();

            // TAGS
            EditorGUILayout.LabelField(new GUIContent("Tags"), this.styles.BoldLabelStyle, null);
            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(300f)))
            {
                this.tag.name = EditorGUILayout.TextField(this.tag.name, GUI.skin.textField, GUILayout.MaxWidth(400f));
                this.tag.color = EditorGUILayout.ColorField(this.tag.color, GUILayout.MaxWidth(400f));
                if (GUILayout.Button("Add tag", GUILayout.MaxWidth(100f)))
                {
                    if (this.tag.name != "New tag")
                    {
                        this.objectEdited.Tags.Add(this.tag);
                        this.tag = new Tag("New tag", Color.red);
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                this.DisplayTags(this.objectEdited.Tags);
            }
            EditorGUILayout.Space();

            // DESCRIPTION
            EditorGUILayout.LabelField(new GUIContent("Description"), this.styles.BoldLabelStyle, null);
            EditorGUILayout.Space();
            this.objectEdited.Description = EditorGUILayout.TextArea(this.objectEdited.Description, GUI.skin.textArea, GUILayout.MinHeight(75f));
            EditorGUILayout.Space();

            // DEPENDENCIES
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(13f);
                using (new EditorGUILayout.VerticalScope(GUI.skin.textArea, GUILayout.ExpandWidth(false)))
                {
                    this.foldoutDependencies = EditorGUILayout.Foldout(this.foldoutDependencies, new GUIContent("Dependencies of the module"));
                    EditorGUILayout.Space();
                    if (this.foldoutDependencies)
                    {
                        if (this.displayDependencies)
                        {
                            this.dependenciesEditors.Display();
                        }
                        else
                        {
                            EditorGUILayout.LabelField(new GUIContent("No modules found. You can't add dependencies for this module."), this.styles.BoldLabelStyle, null);
                        }
                    }
                }
            }
            EditorGUILayout.Space();
        }

        void DisplayTags(IList<Tag> tags)
        {
            Tag tag;
            Color previousBackgroundColor;

            for (int i = tags.Count - 1; i >= 0; i--)
            {
                tag = tags[i];
                previousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = tag.color;
                EditorGUILayout.LabelField(tag.name, EditorStyles.miniButton, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(150f));
                GUI.backgroundColor = previousBackgroundColor;
                if (GUILayout.Button("-", GUILayout.MaxWidth(15f)))
                {
                    tags.RemoveAt(i);
                }
            }
        }
    }
}

