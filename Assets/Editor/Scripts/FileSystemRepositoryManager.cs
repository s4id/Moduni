//
//  FileSystemRepositoryManager.cs
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
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using LibGit2Sharp;
using System.IO;

namespace Moduni
{
    public class FileSystemRepositoryManager : IRepositoryManager
    {
        private FileSystemRepositoryManagerSettings settings;

        public FileSystemRepositoryManager()
        {
        }

        #region IRepositoryManager implementation

        public string Name
        {
            get
            {
                return this.settings.name;
            }
        }

        public Color Color
        {
            get
            {
                return this.settings.color;
            }
        }

        public ARepositoryManagerSettings Settings
        {
            set
            {
                this.settings = (FileSystemRepositoryManagerSettings)value;
            }
        }

        public Task<ISourceControlRepository> CreateRepository(string name)
        {
            return Task.Factory.StartNew<ISourceControlRepository>(() =>
                {
                    if (string.IsNullOrEmpty(name))
                        throw new ArgumentException("The name of the repository should not be empty.");
                    string actualPath = Repository.Init(Path.Combine(this.settings.folderPath, name + ".git"), true);
                    ISourceControlRepository repository = new GitRepository(actualPath);
                    return repository;
                });
        }

        public Task DeleteRepository(ISourceControlRepository repositoryToDelete)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ISourceControlRepository>> GetRepositories()
        {
            return Task.Factory.StartNew<IEnumerable<ISourceControlRepository>>(() =>
                {
                    string[] repositoriesPaths = Directory.GetDirectories(this.settings.folderPath, "*.git", SearchOption.TopDirectoryOnly);
                    ISourceControlRepository[] repositories = new ISourceControlRepository[repositoriesPaths.Length];
                    for (int i = 0; i < repositoriesPaths.Length; i++)
                    {
                        repositories[i] = new GitRepository(repositoriesPaths[i]);
                    }
                    return repositories;
                });
        }

        public Task<IEnumerable<string>> GetFileTextContentFromRepositories(string relativePathInRepository)
        {
            return Task.Factory.StartNew<IEnumerable<string>>(() =>
                {
                    if (string.IsNullOrEmpty(relativePathInRepository))
                        throw new ArgumentException("The path to the file to be retrieved should not be empty.");
                    Task<IEnumerable<ISourceControlRepository>> repositoriesResult = this.GetRepositories();
                    repositoriesResult.Wait();
                    IEnumerable<ISourceControlRepository> repositories = repositoriesResult.Result;
                    List<string> modulesMetadata = new List<string>();
                    foreach (ISourceControlRepository repository in repositories)
                    {
                        modulesMetadata.Add(repository.GetFileContent(relativePathInRepository));
                    }
                    return modulesMetadata;
                });
        }

        #endregion
    }
}
