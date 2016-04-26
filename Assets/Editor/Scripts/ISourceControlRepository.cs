//
//  ISourceControlRepository.cs
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
    using System.Text.RegularExpressions;

    /// <summary>
    /// A repository of source code that is controlled and versioned by a SCM system (Git, Mercurial, SVN...).
    /// </summary>
    public interface ISourceControlRepository : IDisposable
    {
        /// <summary>
        /// Gets all the branches of this repository including the remote ones.
        /// </summary>
        /// <value>The branches of this repository including the remote ones.</value>
        IEnumerable<string> Branches { get; }

        /// <summary>
        /// Gets the name of the current branch that the repository points to.
        /// </summary>
        /// <value>The name of the current branch.</value>
        string CurrentBranch { get; }

        /// <summary>
        /// Gets the reference of the commit to which the repository is currently pointing at.
        /// </summary>
        /// <value>The reference of the commit to which the repository is currently pointing at.</value>
        string CurrentCommit { get; }

        /// <summary>
        /// Gets all the files of the repository except the ones that are ignored.
        /// </summary>
        /// <value>The files of the repository.</value>
        IEnumerable<RepositoryFile> Files { get; }

        /// <summary>
        /// Gets the reference of the first commit chronologically in this repository.
        /// </summary>
        /// <value>The reference of the first commit chronologically in this repository.</value>
        string FirstCommitReference { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is a centralized repository.
        /// </summary>
        /// <value><c>true</c> if this instance is a centralized repository; otherwise, <c>false</c>.</value>
        bool IsBare { get; }

        /// <summary>
        /// Gets a value indicating whether the repository has changed since the last commit.
        /// </summary>
        /// <value><c>true</c> if this repository has changed since the last commit; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets a value indicating whether the repository is pointing to a valid working copy or not.
        /// </summary>
        /// <value><c>true</c> if the repository is pointing to a valid working copy; otherwise, <c>false</c>.</value>
        bool IsRepositoryInitialized { get; }

        /// <summary>
        /// Gets the list of the local branches created in this repository.
        /// </summary>
        /// <value>The list of the local branches created in this repository.</value>
        IEnumerable<string> LocalBranches { get; }

        /// <summary>
        /// Gets the name of the repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        string Name { get; }

        /// <summary>
        /// Gets or sets the path to the origin remote repository.
        /// </summary>
        /// <value>The path to the origin remote repository.</value>
        string RemoteOriginURL { get; set; }

        /// <summary>
        /// Gets the url where the repository is located.
        /// </summary>
        /// <value>The url to the repository.</value>
        string RepositoryURL { get; }

        /// <summary>
        /// Adds a new tag to the head commit whose name is specified as an argument.
        /// </summary>
        /// <param name="newTagName">The name of the new tag.</param>
        void AddTag(string newTagName);

        /// <summary>
        /// Adds a new submodule to the repository.
        /// </summary>
        /// <param name="submoduleRepositoryURL">The URL of the repository that will be added as a submodule.</param>
        /// <param name="submoduleRelativePath">The path relative to the repository where the submodule will be cloned.</param>
        /// <returns>The repository of the submodule.</returns>
        ISourceControlRepository AddSubmodule(string submoduleRepositoryURL, string submoduleRelativePath);

        /// <summary>
        /// Switch the head to the reference specified in the argument. It also changes the property <see cref="CurrentBranch"/>.
        /// </summary>
        /// <param name="referenceName">The name of the reference to switch the head to.</param>
        void CheckoutReference(string referenceName);

        /// <summary>
        /// Creates a new commit with the files inside the staging area.
        /// </summary>
        /// <param name="message">The message to describe the modifications done by the commit.</param>
        void Commit(string message);

        /// <summary>
        /// Checks if this repository contains a branch whose name is specified as argument.
        /// </summary>
        /// <returns><c>true</c>, if the repository contains the branch, <c>false</c> otherwise.</returns>
        /// <param name="branchName">The name of the branch that the method checks the existence of.</param>
        bool ContainsBranch(string branchName);

        /// <summary>
        /// Creates a new branch whose name is specified in the argument.
        /// </summary>
        /// <param name="branchName">The name of the branch that the method creates.</param>
        void CreateBranch(string branchName);

        /// <summary>
        /// Deletes the working copy of this repository. It also releases any native handle of the repository.
        /// </summary>
        void DeleteWorkingCopy();

        /// <summary>
        /// Fetch all the new commits from all the remote repositories.
        /// </summary>
        void FetchAll();

        /// <summary>
        /// Finds the tags inside the repository matching the regular expression specified as argument.
        /// </summary>
        /// <returns>The tags found that match the regular expression.</returns>
        /// <param name="regexToMatch">The regular expression the tags must match.</param>
        IEnumerable<string> FindTags(Regex regexToMatch);

        /// <summary>
        /// Gets the author of the commit whose reference is specified in the argument.
        /// </summary>
        /// <returns>The author's signature for the commit.</returns>
        /// <param name="commitReference">The reference of the commit.</param>
        CommitSignature? GetCommitSignature(string commitReference);

        /// <summary>
        /// Retrieves the content of the file whose path is specified as an argument.
        /// </summary>
        /// <returns>The content of the file.</returns>
        /// <param name="relativePathToFile">The relative path to the file whose content will be retrieved.</param>
        /// <param name="branchVersion">The branch version from which the file is retrieved. If the argument is not provided, it is retrieved from the current branch.</param>
        string GetFileContent(string relativePathToFile, BranchVersion branchVersion = default(BranchVersion));

        /// <summary>
        /// Gets the nearest tag that corresponds to the reference of the commit sent in the argument.
        /// </summary>
        /// <returns>The nearest tag from the commit.</returns>
        /// <param name="commitReference">The reference of the commit.</param>
        string GetNearestTagFromCommit(string commitReference);

        /// <summary>
        /// Gets the relative path to the submodule whose full path is specified as argument.
        /// </summary>
        /// <returns>The relative path to the submodule.</returns>
        /// <param name="fullPath">The full path to the submodule.</param>
        string GetSubmoduleRelativePath(string fullPath);

        /// <summary>
        /// Creates a new working copy of this repository.
        /// </summary>
        /// <returns>The new working copy created.</returns>
        /// <param name="path">The local path where the new working copy should be created.</param>
        ISourceControlRepository InitOrCloneRepository(string path);

        /// <summary>
        /// Moves the submodule situated at <paramref name="oldRelativePath"/> to <paramref name="newRelativePath"/>.
        /// It also changes the file ".gitmodules" with the new path and commit the modifications.
        /// </summary>
        /// <param name="oldRelativePath">The relative path to the current location of the submodule.</param>
        /// <param name="newRelativePath">The relative path to the new location of the submodule.</param>
        void MoveSubmodule(string oldRelativePath, string newRelativePath);

        /// <summary>
        /// Moves the repository to the path specified as argument.
        /// </summary>
        /// <param name="pathToMoveTo">The new path of the repository.</param>
        void MoveTo(string pathToMoveTo);

        /// <summary>
        /// Pull the new commits inside the local branch from the branch tracked in the remote repository.
        /// </summary>
        void Pull();

        /// <summary>
        /// Push the new commits of the local branch to the branch tracked in the remote repository.
        /// </summary>
        void Push();

        /// <summary>
        /// Removes the file whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to be removed.</param>
        /// <param name="removeFromWorkingDirectory">If set to <c>true</c>, it removes the file from the working directory.</param>
        void RemoveFile(string relativePath, bool removeFromWorkingDirectory);

        /// <summary>
        /// Removes the submodule whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the submodule to be removed.</param>
        /// <param name="forceRemoval">If set to <c>true</c>, it deletes the submodule even if it has local modifications.</param>
        void RemoveSubmodule(string relativePath, bool forceRemoval);

        /// <summary>
        /// Adds all the files inside the repository that have changed since the last commit to the staging area for the next commit.
        /// </summary>
        void StageAllChanges();

        /// <summary>
        /// Stages the file whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to be staged.</param>
        void StageFile(string relativePath);
    }
}