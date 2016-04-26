//
//  ModulesToggleGroup.cs
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
using UnityEngine;
using System.Linq;

namespace Moduni.UI
{
    public class ModulesToggleGroup : ASelector<IModule>
    {
        private ASelector<IModule> moduleSelectedSelector;
        private IEnumerable<ASelector<IModule>> selectors;
        private GUIStyles styles;

        public IEnumerable<ASelector<IModule>> Selectors
        {
            set
            {
                if (this.selectors != null)
                {
                    foreach (ASelector<IModule> selector in this.selectors)
                    {
                        selector.OnSelected -= this.OnModuleSelected;
                        selector.OnDeselected -= this.OnModuleDeselected;
                    }
                }
                this.selectors = value;
                foreach (ASelector<IModule> selector in this.selectors)
                {
                    selector.OnSelected += this.OnModuleSelected;
                    selector.OnDeselected += this.OnModuleDeselected;
                }
            }
        }

        public ModulesToggleGroup(IEnumerable<ASelector<IModule>> selectors)
        {
            this.Selectors = selectors;
        }

        void OnModuleSelected(IModule module)
        {
            if (this.moduleSelectedSelector != null)
            {
                this.moduleSelectedSelector.Select(false);
            }
            this.ObjectSelected = module;
            this.moduleSelectedSelector = this.selectors.Single((ASelector<IModule> selector) => selector.ObjectSelected == module);
        }

        void OnModuleDeselected(IModule module)
        {
            this.ObjectSelected = null;
            this.moduleSelectedSelector = null;
        }

        #region implemented abstract members of ASelector

        public override void Display()
        {
            foreach (ASelector<IModule> selector in this.selectors)
            {
                selector.Display();
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

