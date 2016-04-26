//
//  ARepositoryManagerSettingsEditor.cs
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
    public abstract class ARepositoryManagerSettingsEditor : AEditor<ARepositoryManagerSettings>
    {
        protected bool isNameEditable = true;

        protected abstract void DisplayDerivedFields();

        public override void Display()
        {
            using (new EditorGUILayout.HorizontalScope(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).box, GUILayout.MaxWidth(700f)))
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(200f));
                        if (this.isNameEditable)
                        {
                            this.objectEdited.name = EditorGUILayout.TextField(this.objectEdited.name);
                        }
                        else
                        {
                            EditorGUILayout.LabelField(this.objectEdited.name);
                        }
                    }

                    EditorGUILayout.Space();

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Color Identification", EditorStyles.boldLabel, GUILayout.MaxWidth(200f));
                        this.objectEdited.color = EditorGUILayout.ColorField(this.objectEdited.color, null);
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    this.DisplayDerivedFields();
                }
            }
        }
    }
}

