//
//  SourceControlRepositoryFactory.cs
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
using LibGit2Sharp;
using UnityEngine;

namespace Moduni
{
    public class SourceControlRepositoryFactory
    {
        public SourceControlRepositoryFactory()
        {
        }

        public ISourceControlRepository CreateRepository(string workingDirectoryPath)
        {
            if (!string.IsNullOrEmpty(workingDirectoryPath))
            {
                return new GitRepository(workingDirectoryPath);
            }
            return null;
        }

        public ISourceControlRepository DiscoverRepository(string path)
        {
            string repositoryPath = this.DiscoverRepositoryPath(path);
            return this.CreateRepository(repositoryPath);

        }

        public string DiscoverRepositoryPath(string path)
        {
            string repositoryPath = Repository.Discover(path);
            if (!string.IsNullOrEmpty(repositoryPath))
            {
                // Remove '.git' from the path to have the path to the working directory
                repositoryPath = repositoryPath.Remove(repositoryPath.Length - 5, 5);
            }
            return repositoryPath;
        }
    }
}

