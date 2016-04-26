//
//  MessageBoard.cs
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

using System.Collections;
using System;

namespace Moduni.UI
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class MessageBoard : IUIElement
    {
        private Stack<Message> messages;
        private Vector2 scrollPosition;

        public MessageBoard()
        {
            this.messages = new Stack<Message>();
        }

        public void AddMessage(Message newMessage)
        {
            this.messages.Push(newMessage);
        }

        public void Display()
        {
            Color previousBackgroundColor;

            if (this.messages.Count != 0)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.Space();
                EditorGUILayout.Separator();
                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(GUI.skin.textArea, GUILayout.MaxHeight(250f), GUILayout.ExpandHeight(false)))
                {
                    EditorGUILayout.LabelField("Output");
                    EditorGUILayout.Space();
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition))
                    {
                        this.scrollPosition = scrollView.scrollPosition;
                        previousBackgroundColor = GUI.backgroundColor;
                        foreach (Message message in this.messages)
                        {
                            GUI.backgroundColor = Color.Lerp(Color.red, Color.white, (float)(DateTimeOffset.Now - message.dateTime).TotalSeconds / 3f);
                            EditorGUILayout.HelpBox(message.Format(), message.type);
                        }
                        GUI.backgroundColor = previousBackgroundColor;
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }
    }
}

