//
//  IModule.cs
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

namespace Moduni
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public interface IModule : IXmlSerializable
    {
        /// <summary>
        /// Occurs when the status of the files of the module are updated.
        /// </summary>
        event GenericEventHandler<IModule, IEnumerable<RepositoryFile>> OnFilesUpdated;
        /// <summary>
        /// Occurs when the versions of the module are updated.
        /// </summary>
        event GenericEventHandler<IModule, IEnumerable<BranchVersion>> OnVersionsUpdated;

        /// <summary>
        /// Gets the details about the original creator of this module.
        /// </summary>
        /// <value>The details about the original creator of this module.</value>
        CommitSignature Creator { get; }

        /// <summary>
        /// Gets or sets the current branch version that this module is using.
        /// </summary>
        /// <value>The current branch version that this module is using.</value>
        BranchVersion CurrentBranchVersion { get; set; }

        /// <summary>
        /// Sets the current state of the module with the value provided.
        /// </summary>
        /// <value>The new state of the module.</value>
        ModuleState CurrentState { set; }

        /// <summary>
        /// Gets the dependencies of the module.
        /// </summary>
        /// <value>The dependencies of the module.</value>
        IList<Dependency> Dependencies { get; }

        /// <summary>
        /// Gets or sets the description of the module. It should include the purpose of this module, what it is doing.
        /// </summary>
        /// <value>The description of the module, its purpose, what it is doing.</value>
        string Description { get; set; }

        /// <summary>
        /// Gets the path and the status (unchanged, modified, new...) of all the files of the module.
        /// </summary>
        /// <value>The path and the status (unchanged, modified, new...) of all the files of the module.</value>
        IEnumerable<RepositoryFile> Files { get; }

        /// <summary>
        /// Gets a value indicating whether this module has changed since it has been imported or published.
        /// </summary>
        /// <value><c>true</c> if this module has changed since it has been imported or published; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets the last test report of the module created by the continuous integration system.
        /// </summary>
        /// <value>The last test report of the module created by the continuous integration system.</value>
        TestsReport LastTestReport { get; }

        /// <summary>
        /// Gets the name of the file where the metadata about the module are stored.
        /// </summary>
        /// <value>The name of the file where the metadata about the module are stored.</value>
        string MetadataFilename { get; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the path to the module inside the project.
        /// </summary>
        /// <value>The path to the module inside the project.</value>
        string Path { get; set; }

        /// <summary>
        /// Gets the URL of the repository.
        /// </summary>
        /// <value>The URL of the repository.</value>
        string RepositoryURL { get; }

        /// <summary>
        /// Gets the tags associated with this module.
        /// </summary>
        /// <value>The tags associated with this module.</value>
        IList<Tag> Tags { get; }

        /// <summary>
        /// Gets or sets the technology readiness level of the module (a level to evaluate the maturity of a technology created by the NASA).
        /// </summary>
        /// <value>The technology readiness level of the module (a level to evaluate the maturity of a technology created by the NASA).</value>
        int TRL { get; set; }

        /// <summary>
        /// Gets the unique identifier of the module.
        /// </summary>
        /// <value>The unique identifier of the module.</value>
        Guid UUID { get; }

        /// <summary>
        /// Gets the details about the person that has created the current version of the module.
        /// </summary>
        /// <value>The details about the person that has created the current version of the module.</value>
        CommitSignature VersionAuthor { get; }

        /// <summary>
        /// Gets all the versions available for this module.
        /// </summary>
        /// <value>The versions available for this module.</value>
        IEnumerable<Moduni.BranchVersion> Versions { get; }

        /// <summary>
        /// Browses the documentation of the module by opening it with a third party application.
        /// </summary>
        void BrowseDocumentation();

        /// <summary>
        /// Clones the current state of the module.
        /// </summary>
        /// <returns>The current state of the module.</returns>
        ModuleState CloneState();

        /// <summary>
        /// Checkouts the version specified in the argument. The working copy should be in the same state as it was for this version.
        /// </summary>
        /// <param name="version">The version to checkout.</param>
        void CheckoutBranchVersion(Moduni.BranchVersion version);

        /// <summary>
        /// Creates a clone of the source control repository that this module uses at the path specified in the argument.
        /// </summary>
        /// <returns>The clone of the module's repository.</returns>
        /// <param name="pathToWorkingDirectory">The path to the working directory where the repository should be cloned.</param>
        ISourceControlRepository CloneRepository(string pathToWorkingDirectory);

        /// <summary>
        /// Creates the base branches that a module needs to work correctly.
        /// </summary>
        void CreateBaseBranches();

        /// <summary>
        /// Deletes the handle of the repository.
        /// </summary>
        void DisposeRepository();

        /// <summary>
        /// Publishes all the changes made to the files of the module.
        /// </summary>
        /// <param name="message">The message to explain the changes made for this publication.</param>
        void PublishChanges(string message);

        /// <summary>
        /// Publishes a new version of the module whose number is specified as argument.
        /// </summary>
        /// <param name="newVersion">The number to identify the new version.</param>
        void PublishVersion(Moduni.BranchVersion newVersion);

        /// <summary>
        /// Saves the metadata of the module.
        /// </summary>
        void SaveMetadata();
    }
}