//
//  GUIStyles.cs
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

namespace Moduni.UI
{
    public class GUIStyles
    {
        private GUIStyle boldLabelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle fileStatusStyle;
        private GUIStyle toolbarStyle;

        public GUIStyles()
        {
        }

        public GUIStyle BoldLabelStyle
        {
            get
            {
                if (this.boldLabelStyle == null)
                {
                    this.boldLabelStyle = new GUIStyle(GUI.skin.label);
                    this.boldLabelStyle.fontSize = 12;
                    this.boldLabelStyle.fontStyle = FontStyle.Bold;
                }
                return this.boldLabelStyle;
            }
        }

        public GUIStyle ButtonStyle
        {
            get
            {
                if (this.buttonStyle == null)
                {
                    this.buttonStyle = new GUIStyle(EditorStyles.toolbarButton);
                    this.buttonStyle.fontSize = 12;
                    this.buttonStyle.stretchHeight = true;
                    this.buttonStyle.fixedHeight = 50f;
                }
                return this.buttonStyle;
            }
        }

        public GUIStyle FileStatusStyle
        {
            get
            {
                if (this.fileStatusStyle == null)
                {
                    this.fileStatusStyle = new GUIStyle(EditorStyles.label);
                    this.fileStatusStyle.clipping = TextClipping.Overflow;
                    this.fileStatusStyle.alignment = TextAnchor.MiddleLeft;
                    this.fileStatusStyle.fontSize = 14;
                    this.fileStatusStyle.stretchHeight = true;
                    this.fileStatusStyle.stretchWidth = true;
                }
                return this.fileStatusStyle;
            }
        }

        public GUIStyle ToolbarStyle
        {
            get
            {
                if (this.toolbarStyle == null)
                {
                    this.toolbarStyle = new GUIStyle(GUI.skin.FindStyle("toolbarbutton"));
                    this.toolbarStyle.fontSize = 14;
                    this.toolbarStyle.fixedHeight = 25f;
                }
                return this.toolbarStyle;
            }
        }
    }
}

