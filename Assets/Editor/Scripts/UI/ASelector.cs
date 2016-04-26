//
//  ASelector.cs
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

namespace Moduni.UI
{
    public abstract class ASelector<T> : IUIElement
    {
        protected T objectSelected;

        public delegate void OnSelectedHandler(T objectSelected);

        public event OnSelectedHandler OnSelected;
        public event OnSelectedHandler OnDeselected;

        public abstract bool IsDisabled { set; }

        public T ObjectSelected
        { 
            get
            {
                return this.objectSelected;
            }
            protected set
            {
                T previousValue = this.objectSelected;
                this.objectSelected = value;
                if (value == null)
                {
                    if (this.OnDeselected != null)
                        this.OnDeselected(previousValue);
                }
                else
                {
                    if (this.OnSelected != null)
                        this.OnSelected(this.objectSelected);
                }
            }
        }

        public abstract void Display();

        public abstract void Select(bool selected);
    }
}

