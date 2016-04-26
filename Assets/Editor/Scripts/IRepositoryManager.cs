//
//  IRepositoryManager.cs
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

using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Moduni
{
    public interface IRepositoryManager
    {
        /// <summary>
        /// The 'friendly' name of the repository manager.
        /// </summary>
        /// <value>The 'friendly' name of the repository manager.</value>
        string Name { get; }

        /// <summary>
        /// The color used to represent this repository manager in the user interface.
        /// </summary>
        /// <value>The color used to represent this repository manager in the user interface.</value>
        Color Color { get; }

        /// <summary>
        /// Sets the settings of the repository manager.
        /// </summary>
        /// <value>The settings of the repository manager.</value>
        ARepositoryManagerSettings Settings { set; }

        /// <summary>
        /// Creates a new repository whose name is specified in the argument.
        /// </summary>
        /// <returns>The new repository created.</returns>
        /// <param name="name">The name of the new repository.</param>
        Task<ISourceControlRepository> CreateRepository(string name);

        /// <summary>
        /// Deletes the repository specified in the argument.
        /// </summary>
        /// <param name="repositoryToDelete">The repository to be deleted.</param>
        Task DeleteRepository(ISourceControlRepository repositoryToDelete);

        /// <summary>
        /// Lists all the repositories of the manager.
        /// </summary>
        /// <returns>The result of the request: whether it is terminated or not and a field to get the list of the repositories.</returns>
        Task<IEnumerable<ISourceControlRepository>> GetRepositories();
    }
}