﻿//
//  Dependency.cs
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

namespace Moduni
{
    public enum DependencyRelationship
    {
        MINIMUM,
        IN_BETWEEN,
        EXACT
    }

    public class Dependency : ICloneable
    {
        private Moduni.BranchVersion maximumVersion;
        private Moduni.BranchVersion minimumVersion;
        private DependencyRelationship relationship;
        private Guid uuidModuleRequired;

        public Moduni.BranchVersion MinimumVersion
        {
            get
            {
                return this.minimumVersion;
            }
            set
            {
                this.minimumVersion = value;
            }
        }

        public DependencyRelationship Relationship
        {
            get
            {
                return relationship;
            }
            set
            {
                relationship = value;
            }
        }

        public Guid UUIDModuleRequired
        {
            get
            {
                return this.uuidModuleRequired;
            }
            set
            {
                this.uuidModuleRequired = value;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

