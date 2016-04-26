//
//  Colors.cs
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

namespace Moduni.UI
{
    public static class Colors
    {
        public static Color Celeste
        {
            get
            {
                return new Color(0.698f, 1f, 1f, 1f);
            }
        }

        public static Color ElectricBlue
        {
            get
            {
                return new Color(0.172f, 0.458f, 1f, 1f);
            }
        }

        public static Color LightRed
        {
            get
            {
                return new Color(1f, 0.5f, 0.5f, 1f);
            }
        }

        public static Color LightGreen
        {
            get
            {
                return new Color(0.6f, 1f, 0.6f, 1f);
            }
        }

        public static Color LightBlue
        {
            get
            {
                return new Color(0.6f, 0.6f, 1f, 1f);
            }
        }

        public static Color LightPink
        {
            get
            {
                return new Color(1f, 0.75f, 1f, 1f);
            }
        }

        public static Color Orange
        {
            get
            {
                return new Color(1f, 0.82f, 0.5f, 1f);
            }
        }

        public static Color SkyBlue
        {
            get
            {
                return new Color(0.5f, 0.902f, 1f, 1f);
            }
        }
    }
}

