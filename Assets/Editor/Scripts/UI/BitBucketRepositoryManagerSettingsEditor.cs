//
//  BitBucketRepositoryManagerSettingsEditor.cs
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
using UnityEditor;
using UnityEngine;

namespace Moduni.UI
{
    public class BitBucketRepositoryManagerSettingsEditor : ARepositoryManagerSettingsEditor
    {
        public BitBucketRepositoryManagerSettingsEditor()
        {
            this.isNameEditable = false;
        }

        protected override void DisplayDerivedFields()
        {
            BitBucketRepositoryManagerSettings bitBucketRepositoryManagerSettings = (BitBucketRepositoryManagerSettings)this.objectEdited;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Scheme", EditorStyles.boldLabel);
                bitBucketRepositoryManagerSettings.scheme = (BitBucketRepositoryManagerSettings.Scheme)EditorGUILayout.EnumPopup(bitBucketRepositoryManagerSettings.scheme);
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Host", EditorStyles.boldLabel);
                bitBucketRepositoryManagerSettings.host = EditorGUILayout.TextField(bitBucketRepositoryManagerSettings.host);
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Port", EditorStyles.boldLabel);
                bitBucketRepositoryManagerSettings.Port = EditorGUILayout.IntField(bitBucketRepositoryManagerSettings.Port);
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Username", EditorStyles.boldLabel);
                bitBucketRepositoryManagerSettings.username = EditorGUILayout.TextField(bitBucketRepositoryManagerSettings.username);
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Password", EditorStyles.boldLabel);
                string previousPassword = bitBucketRepositoryManagerSettings.DecryptedPassword;
                string newPassword = EditorGUILayout.PasswordField(bitBucketRepositoryManagerSettings.DecryptedPassword);
                if (newPassword != previousPassword)
                {
                    bitBucketRepositoryManagerSettings.DecryptedPassword = newPassword;
                }
            }
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Project key", EditorStyles.boldLabel);
                bitBucketRepositoryManagerSettings.projectKey = EditorGUILayout.TextField(bitBucketRepositoryManagerSettings.projectKey);
            }
            EditorGUILayout.Space();
        }
    }
}

