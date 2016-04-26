//
//  RepositoryManagerSelector.cs
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
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Moduni.UI
{
    public class RepositoryManagerSelector : ASelector<IRepositoryManager>
    {
        private IEnumerable<IRepositoryManager> repositoriesManagers;
        private Vector2 scrollPosition;

        public IEnumerable<IRepositoryManager> RepositoriesManagers
        {
            set
            {
                this.repositoriesManagers = value;
            }
        }

        public RepositoryManagerSelector(IEnumerable<IRepositoryManager> repositoriesManager)
        {
            this.RepositoriesManagers = repositoriesManager;
        }

        #region IUIElement implementation

        public override void Display()
        {
            using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPosition, false, false))
            {
                this.scrollPosition = scrollView.scrollPosition;

                Color previousBackgroundColor = GUI.backgroundColor;
                foreach (IRepositoryManager repositoryManager in this.repositoriesManagers)
                {
                    GUI.backgroundColor = repositoryManager.Color;
                    if (GUILayout.Button(repositoryManager.Name, GUILayout.MinHeight(50f)))
                    {
                        this.ObjectSelected = repositoryManager;
                    }
                    GUI.backgroundColor = previousBackgroundColor;
                }
            }
        }

        public override void Select(bool selected)
        {
        }

        public override bool IsDisabled
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

