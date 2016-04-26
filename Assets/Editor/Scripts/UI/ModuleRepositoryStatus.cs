//
//  ModuleRepositoryStatus.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ModuleRepositoryStatus : IUIElement
    {
        private static Texture2D iconAdded;
        private static Texture2D iconDeleted;
        private static Texture2D iconModified;
        private static Texture2D iconMoved;
        private static Texture2D iconUnchanged;

        private bool foldout;
        private IModule module;
        private IEnumerable<RepositoryFile> repositoryFiles;
        private Vector2 scrollPosition;
        private GUIStyles styles;

        public ModuleRepositoryStatus(IModule module, GUIStyles styles)
        {
            this.module = module;
            this.OnModuleFilesUpdated(module, module.Files);
            this.styles = styles;

            this.module.OnFilesUpdated += this.OnModuleFilesUpdated;
        }

        public void Display()
        {
            if (!this.repositoryFiles.Any())
                return;

            if (ModuleRepositoryStatus.iconAdded == null)
            {
                ModuleRepositoryStatus.iconAdded = (Texture2D)EditorGUIUtility.Load("icon_added.png");
                ModuleRepositoryStatus.iconDeleted = (Texture2D)EditorGUIUtility.Load("icon_deleted.png");
                ModuleRepositoryStatus.iconModified = (Texture2D)EditorGUIUtility.Load("icon_modified.png");
                ModuleRepositoryStatus.iconMoved = (Texture2D)EditorGUIUtility.Load("icon_moved.png");
                ModuleRepositoryStatus.iconUnchanged = (Texture2D)EditorGUIUtility.Load("icon_unchanged.png");
            }

            Texture2D icon = null;

            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false)))
            {
                GUILayout.Space(125f);
                using (new EditorGUILayout.VerticalScope(GUI.skin.textArea, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                {
                    this.foldout = EditorGUILayout.Foldout(this.foldout, "Status");
                    if (this.foldout)
                    {
                        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition, GUILayout.MaxHeight(400f), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false)))
                        {
                            this.scrollPosition = scrollView.scrollPosition;

                            foreach (RepositoryFile repositoryFile in this.repositoryFiles)
                            {
                                switch (repositoryFile.status)
                                {
                                    case RepositoryFileStatus.New:
                                        icon = ModuleRepositoryStatus.iconAdded;
                                        break;
                                    case RepositoryFileStatus.Deleted:
                                        icon = ModuleRepositoryStatus.iconDeleted;
                                        break;
                                    case RepositoryFileStatus.Modified:
                                        icon = ModuleRepositoryStatus.iconModified;
                                        break;
                                    case RepositoryFileStatus.Unchanged:
                                        icon = ModuleRepositoryStatus.iconUnchanged;
                                        break;
                                    case RepositoryFileStatus.Moved:
                                        icon = ModuleRepositoryStatus.iconMoved;
                                        break;
                                }
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    GUILayout.Label(icon, GUILayout.MaxWidth(24), GUILayout.MaxHeight(24));
                                    EditorGUILayout.LabelField(new GUIContent(repositoryFile.path), this.styles.FileStatusStyle);
                                }
                            }
                        }
                    }
                }
            }
        }

        void OnModuleFilesUpdated(IModule module, IEnumerable<RepositoryFile> files)
        {
            this.repositoryFiles = files.Where((RepositoryFile repositoryFile) => !repositoryFile.path.EndsWith(".meta") && repositoryFile.path != this.module.MetadataFilename);
        }
    }
}

