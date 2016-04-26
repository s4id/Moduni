﻿//
//  RepositoryManagerFactory.cs
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

namespace Moduni
{
    public class RepositoryManagerFactory
    {
        public RepositoryManagerFactory()
        {
        }

        public IRepositoryManager CreateRepositoryManagerFromSettings(ARepositoryManagerSettings repositoryManagerSettings)
        {
            IRepositoryManager repositoryManager = null;
            if (repositoryManagerSettings is BitBucketRepositoryManagerSettings)
            {
                repositoryManager = new BitBucketRepositoryManager();
            }
            else if (repositoryManagerSettings is SSHServerRepositoryManagerSettings)
            {
                repositoryManager = new SSHServerRepositoryManager();
            }
            else if (repositoryManagerSettings is FileSystemRepositoryManagerSettings)
            {
                repositoryManager = new FileSystemRepositoryManager();
            }
            else
            {
                throw new NotSupportedException("The repository manager associated with these settings is not supported by the factory.");
            }
            repositoryManager.Settings = repositoryManagerSettings;
            return repositoryManager;
        }
    }
}

