//
//  FileSystemRepositoryManagerSettingsEditor.cs
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

namespace Moduni.UI
{
    public class FileSystemRepositoryManagerSettingsEditor : ARepositoryManagerSettingsEditor
    {
        protected override void DisplayDerivedFields()
        {
            FileSystemRepositoryManagerSettings fileSystemRepositoryManagerSettings = (FileSystemRepositoryManagerSettings)this.objectEdited;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Repositories folder path", EditorStyles.boldLabel, GUILayout.MaxWidth(200f));
                EditorGUILayout.LabelField(fileSystemRepositoryManagerSettings.folderPath, EditorStyles.label);
                if (GUILayout.Button("..."))
                {
                    fileSystemRepositoryManagerSettings.folderPath = EditorUtility.SaveFolderPanel("Select the folder where the repositories will be saved", Application.dataPath, "Repositories");
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.Space();
        }
    }
}

