//
//  GitRepository.cs
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

namespace Moduni
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LibGit2Sharp;

    public class GitRepository : ISourceControlRepository
    {
        private LibGit2Sharp.IRepository gitRepositoryHandle;
        private string remoteOriginURL;

        public GitRepository()
        {
        }

        public GitRepository(string workingDirectoryPath)
        {
            this.gitRepositoryHandle = new Repository(workingDirectoryPath);
        }

        ~GitRepository()
        {
            this.Dispose();
        }

        /// <summary>
        /// Gets all the branches of this repository including the remote ones.
        /// </summary>
        /// <value>The branches of this repository including the remote ones.</value>
        public IEnumerable<string> Branches
        {
            get
            {
                return this.gitRepositoryHandle.Branches.Select<Branch, string>((Branch branch) => branch.FriendlyName); 
            }
        }

        /// <summary>
        /// Gets the name of the current branch that the repository points to.
        /// </summary>
        /// <value>The name of the current branch.</value>
        public string CurrentBranch
        {
            get
            {
                if (this.gitRepositoryHandle != null)
                    return this.gitRepositoryHandle.Head.FriendlyName;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the reference of the commit to which the repository is currently pointing at.
        /// </summary>
        /// <value>The reference of the commit to which the repository is currently pointing at.</value>
        public string CurrentCommit
        {
            get
            {
                if (this.gitRepositoryHandle.Head.Tip != null)
                    return this.gitRepositoryHandle.Head.Tip.Sha;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets all the files of the repository except the ones that are ignored.
        /// If it is a bare repository, it doesn't get any files.
        /// </summary>
        /// <value>The files of the repository.</value>
        public IEnumerable<RepositoryFile> Files
        {
            get
            {
                List<RepositoryFile> repositoryFiles = new List<RepositoryFile>();
                if (!this.gitRepositoryHandle.Info.IsBare)
                {
                    RepositoryStatus repositoryStatus = this.gitRepositoryHandle.RetrieveStatus(new StatusOptions()
                        { 
                            DetectRenamesInIndex = true, 
                            DetectRenamesInWorkDir = true,
                            IncludeUnaltered = true,
                            Show = StatusShowOption.IndexAndWorkDir
                        });
                    repositoryFiles.AddRange(repositoryStatus.Select<StatusEntry,RepositoryFile>((StatusEntry statusEntry) => statusEntry.ToRepositoryFile()).Where((RepositoryFile repositoryFile) => repositoryFile.status != RepositoryFileStatus.Unknown));
                }
                return repositoryFiles;
            }
        }

        /// <summary>
        /// Gets the reference of the first commit chronologically in this repository.
        /// </summary>
        /// <value>The reference of the first commit chronologically in this repository.</value>
        public string FirstCommitReference
        {
            get
            {
                if (this.gitRepositoryHandle.Commits.FirstOrDefault() != null)
                    return this.gitRepositoryHandle.Commits.Last().Sha;
                else
                    return null;
            }
        }

        /// <summary>
        /// Sets the handle of the Git repository.
        /// </summary>
        /// <value>The handle of the Git repository.</value>
        public LibGit2Sharp.IRepository GitRepositoryHandle
        {
            set
            {
                this.gitRepositoryHandle = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a centralized repository.
        /// </summary>
        /// <value>true</value>
        /// <c>false</c>
        public bool IsBare
        {
            get
            {
                return this.gitRepositoryHandle.Info.IsBare;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the repository has changed since the last commit.
        /// </summary>
        /// <value><c>true</c> if this repository has changed since the last commit; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                return this.gitRepositoryHandle.RetrieveStatus().IsDirty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the repository is pointing to a valid working copy or not.
        /// </summary>
        /// <value><c>true</c> if the repository is pointing to a valid working copy; otherwise, <c>false</c>.</value>
        public bool IsRepositoryInitialized
        {
            get
            {
                return this.gitRepositoryHandle != null;
            }
        }

        /// <summary>
        /// Gets the list of the local branches created in this repository.
        /// </summary>
        /// <value>The list of the local branches created in this repository.</value>
        public IEnumerable<string> LocalBranches
        {
            get
            {
                return this.gitRepositoryHandle.Branches.Where((Branch branch) => !branch.IsRemote).Select<Branch,string>((Branch branch) => branch.FriendlyName); 
            }
        }

        /// <summary>
        /// Gets the name of the repository.
        /// </summary>
        /// <value>The name of the repository.</value>
        public string Name
        {
            get
            {
                string url;
                if (!string.IsNullOrEmpty(this.RemoteOriginURL))
                {
                    url = this.remoteOriginURL;
                }
                else if (this.RepositoryURL != null)
                {
                    url = this.RepositoryURL;
                }
                else
                    return null;

                string[] splitURL = url.Split(new string[4]{ "/", "//", "\\", "\\\\" }, StringSplitOptions.RemoveEmptyEntries);
                return splitURL[splitURL.Length - 1];
            }
        }

        /// <summary>
        /// Gets or sets the path to the origin remote repository.
        /// </summary>
        /// <value>The path to the origin remote repository.</value>
        public string RemoteOriginURL
        {
            get
            {
                return this.remoteOriginURL;
            }
            set
            {
                this.remoteOriginURL = value;
                if (this.gitRepositoryHandle != null)
                {
                    Remote originRemote = this.gitRepositoryHandle.Network.Remotes.FirstOrDefault((Remote remote) => remote.Name == "origin");
                    if (originRemote != null)
                    {
                        this.gitRepositoryHandle.Network.Remotes.Update(originRemote, (RemoteUpdater remoteUpdater) =>
                            {
                                remoteUpdater.Url = this.remoteOriginURL;
                                Debug.Log(this.remoteOriginURL);
                            });
                    }
                    else
                    {
                        this.gitRepositoryHandle.Network.Remotes.Add("origin", this.remoteOriginURL);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the url where the repository is located.
        /// </summary>
        /// <value>The url to the repository.</value>
        public string RepositoryURL
        {
            get
            {
                if (this.IsRepositoryInitialized)
                {
                    if (this.gitRepositoryHandle.Info.WorkingDirectory != null)
                        return this.gitRepositoryHandle.Info.WorkingDirectory;
                    else
                        return this.gitRepositoryHandle.Info.Path;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Adds a new tag to the head commit whose name is specified as an argument.
        /// </summary>
        /// <param name="newTagName">The name of the new tag.</param>
        public void AddTag(string newTagName)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            LibGit2Sharp.Tag tag = this.gitRepositoryHandle.ApplyTag(newTagName, this.gitRepositoryHandle.Config.BuildSignature(new DateTimeOffset()), "New version: " + newTagName);
            this.gitRepositoryHandle.Network.Push(this.gitRepositoryHandle.Network.Remotes["origin"], tag.CanonicalName);
        }

        /// <summary>
        /// Adds a new submodule to the repository.
        /// </summary>
        /// <param name="submoduleRepositoryURL">The URL of the repository that will be added as a submodule.</param>
        /// <param name="submoduleRelativePath">The path relative to the repository where the submodule will be cloned.</param>
        /// <returns>The repository of the submodule.</returns>
        public ISourceControlRepository AddSubmodule(string submoduleRepositoryURL, string submoduleRelativePath)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            this.gitRepositoryHandle.Submodules.Add(submoduleRepositoryURL, submoduleRelativePath);
            ISourceControlRepository submoduleRepository = new GitRepository(submoduleRelativePath);
            return submoduleRepository;
            
        }

        /// <summary>
        /// Checkouts the reference.
        /// </summary>
        /// <param name="referenceName">Reference name.</param>
        public void CheckoutReference(string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
                throw new ArgumentException("The name of the branch to switch to is empty.");

            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (!this.gitRepositoryHandle.Info.IsBare)
            {
                try
                {
                    this.gitRepositoryHandle.Checkout(referenceName, new CheckoutOptions(){ CheckoutModifiers = CheckoutModifiers.None, CheckoutNotifyFlags = CheckoutNotifyFlags.Conflict });
                }
                catch (NotFoundException)
                {
                    throw new ModuniException("The reference to checkout doesn't exist.");
                }
                catch (CheckoutConflictException)
                {
                    throw new ModuniException("Local changes prevent checkout, please commit your changes or discard them.");
                }
            }
        }

        /// <summary>
        /// Creates a new commit with the files inside the staging area.
        /// </summary>
        /// <param name="message">The message to describe the modifications done by the commit.</param>
        public void Commit(string message)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (this.gitRepositoryHandle.Index.Count == 0)
                throw new ModuniException("The commit is empty.");

            Signature signature = this.gitRepositoryHandle.Config.BuildSignature(DateTimeOffset.UtcNow);
            this.gitRepositoryHandle.Commit(message, signature, signature);
        }

        /// <summary>
        /// Checks if this repository contains a branch whose name is specified as argument.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="branchName">The name of the branch that the method checks the existence of.</param>
        public bool ContainsBranch(string branchName)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (this.gitRepositoryHandle.Branches.FirstOrDefault((Branch branch) => branch.FriendlyName == branchName) != null)
                return true;
            return false;
        }

        /// <summary>
        /// Creates a new branch whose name is specified in the argument.
        /// </summary>
        /// <param name="branchName">The name of the branch that the method creates.</param>
        public void CreateBranch(string branchName)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            Branch newBranch = this.gitRepositoryHandle.CreateBranch(branchName);
            if (this.remoteOriginURL != null)
            {
                this.gitRepositoryHandle.Branches.Update(newBranch, (BranchUpdater branchUpdater) => branchUpdater.Remote = "origin", (BranchUpdater branchUpdater) => branchUpdater.UpstreamBranch = "refs/heads/" + branchName);
                this.gitRepositoryHandle.Network.Push(newBranch);
            }
        }

        /// <summary>
        /// Deletes the working copy of this repository. It also releases any native handle of the repository.
        /// </summary>
        public void DeleteWorkingCopy()
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            string repositoryURL = this.RepositoryURL;
            this.Dispose();
            DirectoryExtensions.DeleteCompletely(repositoryURL);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Moduni.GitRepository"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Moduni.GitRepository"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="Moduni.GitRepository"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="Moduni.GitRepository"/> so
        /// the garbage collector can reclaim the memory that the <see cref="Moduni.GitRepository"/> was occupying.</remarks>
        public void Dispose()
        {
            foreach (string file in System.IO.Directory.GetFiles(this.RepositoryURL,"*",System.IO.SearchOption.AllDirectories))
            {
                System.IO.File.SetAttributes(file, System.IO.FileAttributes.Normal);
            }
            if (this.gitRepositoryHandle != null)
                this.gitRepositoryHandle.Dispose();
            this.gitRepositoryHandle = null;
            
        }

        /// <summary>
        /// Fetch all the new commits from all the remote repositories.
        /// </summary>
        public void FetchAll()
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            foreach (Remote remote in this.gitRepositoryHandle.Network.Remotes)
            {
                this.gitRepositoryHandle.Network.Fetch(remote, new FetchOptions(){ TagFetchMode = TagFetchMode.All });
            }
        }

        /// <summary>
        /// Finds the tags inside the repository matching the regular expression specified as argument.
        /// </summary>
        /// <returns>The tags found that match the regular expression.</returns>
        /// <param name="regexToMatch">The regular expression the tags must match.</param>
        public IEnumerable<string> FindTags(System.Text.RegularExpressions.Regex regexToMatch)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            return this.gitRepositoryHandle.Tags.Where((LibGit2Sharp.Tag tag) => regexToMatch.IsMatch(tag.FriendlyName)).Select<LibGit2Sharp.Tag,string>((LibGit2Sharp.Tag tag) => tag.FriendlyName);
        }

        /// <summary>
        /// Gets the author of the commit whose reference is specified in the argument.
        /// </summary>
        /// <returns>The author's signature for the commit.</returns>
        /// <param name="commitReference">The reference of the commit.</param>
        public CommitSignature? GetCommitSignature(string commitReference)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            Commit commit = this.gitRepositoryHandle.Lookup<Commit>(commitReference);
            if (commit != null)
            {
                return commit.Author;
            }
            else
            {
                LibGit2Sharp.Tag tag = this.gitRepositoryHandle.Tags.FirstOrDefault((LibGit2Sharp.Tag tagSearched) => tagSearched.FriendlyName == commitReference);
                if (tag != null)
                {
                    commit = this.gitRepositoryHandle.Lookup<Commit>(tag.Target.Id);
                    if (commit != null)
                        return commit.Author;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves the content of the file whose path is specified as an argument.
        /// </summary>
        /// <returns>The content of the file.</returns>
        /// <param name="relativePathToFile">The relative path to the file whose content will be retrieved.</param>
        /// <param name="branchVersion">The branch version from which the file is retrieved. If the argument is not provided, it is retrieved from
        /// the current branch.</param>
        public string GetFileContent(string relativePathToFile, BranchVersion branchVersion = default(BranchVersion))
        {
            string branchVersionName = branchVersion.ToString();
            if (branchVersionName == null)
                branchVersionName = "master";
            Commit commit = null;
            if (branchVersion.IsVersion)
            {
                LibGit2Sharp.Tag tag = this.gitRepositoryHandle.Tags.FirstOrDefault((LibGit2Sharp.Tag tagSearched) => tagSearched.FriendlyName == branchVersionName);
                if (tag != null)
                    commit = this.gitRepositoryHandle.Lookup<Commit>(tag.Target.Id);
            }
            else
                commit = this.gitRepositoryHandle.Lookup<Commit>(branchVersionName);
            if (commit != null)
            {
                Blob fileBlob = (Blob)commit[relativePathToFile].Target;
                return fileBlob.GetContentText();
            }
            else
                return null;
        }

        /// <summary>
        /// Gets the nearest tag that corresponds to the reference of the commit sent in the argument.
        /// </summary>
        /// <returns>The nearest tag from the commit.</returns>
        /// <param name="commitReference">The reference of the commit.</param>
        public string GetNearestTagFromCommit(string commitReference)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            Commit commit = this.gitRepositoryHandle.Lookup<Commit>(commitReference);
            if (commit != null)
                return this.gitRepositoryHandle.Describe(commit);
            else
                return null;
        }

        /// <summary>
        /// Gets the relative path to the submodule whose full path is specified as argument.
        /// </summary>
        /// <returns>The relative path to the submodule.</returns>
        /// <param name="fullPath">The full path to the submodule.</param>
        public string GetSubmoduleRelativePath(string fullPath)
        {
            return new System.IO.DirectoryInfo(fullPath).FullName.Replace(new System.IO.DirectoryInfo(this.RepositoryURL).FullName, "").Replace('\\', '/');
        }

        /// <summary>
        /// Creates a new working copy of this repository.
        /// </summary>
        /// <returns>The new working copy created.</returns>
        /// <param name="path">The local path where the new working copy should be created.</param>
        public ISourceControlRepository InitOrCloneRepository(string path)
        {
            string actualRepositoryPath;
            if (this.IsRepositoryInitialized)
            {
                try
                {
                    actualRepositoryPath = Repository.Clone(this.RepositoryURL, path, new CloneOptions(){ IsBare = false });
                    ISourceControlRepository newRepository = new GitRepository(actualRepositoryPath);
                    newRepository.RemoteOriginURL = this.RepositoryURL;
                    return newRepository;
                }
                catch (NameConflictException)
                {
                    actualRepositoryPath = Repository.Init(path, false);
                    ISourceControlRepository newRepository = new GitRepository(actualRepositoryPath);
                    newRepository.RemoteOriginURL = this.RepositoryURL;
                    newRepository.FetchAll();
                    try
                    {
                        newRepository.CheckoutReference("master");
                    }
                    catch (ModuniException)
                    {
                    }
                    return newRepository;
                }
            }
            else
            {
                if (this.remoteOriginURL != null)
                {
                    try
                    {
                        actualRepositoryPath = Repository.Clone(this.remoteOriginURL, path, new CloneOptions(){ IsBare = false });
                        this.gitRepositoryHandle = new Repository(actualRepositoryPath);
                    }
                    catch (NameConflictException)
                    {
                        actualRepositoryPath = Repository.Init(path, false);
                        this.gitRepositoryHandle = new Repository(actualRepositoryPath);
                        this.RemoteOriginURL = this.remoteOriginURL;
                        this.FetchAll();
                        try
                        {
                            this.CheckoutReference("origin/master");
                        }
                        catch (ModuniException)
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        actualRepositoryPath = Repository.Init(path, false);
                    }
                    catch (LibGit2SharpException)
                    {
                        throw new ModuniException("The path to the working copy is invalid.");
                    }
                    this.gitRepositoryHandle = new Repository(actualRepositoryPath);
                }
                return this;
            }
        }

        /// <summary>
        /// Moves the submodule situated at <paramref name="oldRelativePath"/> to <paramref name="newRelativePath"/>.
        /// It also changes the file ".gitmodules" with the new path and commit the modifications.
        /// </summary>
        /// <param name="oldRelativePath">The relative path to the current location of the submodule.</param>
        /// <param name="newRelativePath">The relative path to the new location of the submodule.</param>
        public void MoveSubmodule(string oldRelativePath, string newRelativePath)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            string oldAbsolutePath = System.IO.Path.Combine(this.RepositoryURL, oldRelativePath);
            string newAbsolutePath = System.IO.Path.Combine(this.RepositoryURL, newRelativePath);
            string submoduleURL = this.gitRepositoryHandle.Submodules[oldRelativePath].Url;
            string oldGitDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(this.RepositoryURL, ".git/Modules"), oldRelativePath);
            string gitDirectoryPath = System.IO.Path.Combine(System.IO.Path.Combine(this.RepositoryURL, ".git/Modules"), newRelativePath);
            DirectoryExtensions.Copy(oldAbsolutePath, newAbsolutePath, true);
            DirectoryExtensions.Copy(oldGitDirectoryPath, gitDirectoryPath, true);
            this.RemoveSubmodule(oldRelativePath, true);
            Configuration gitModulesConfiguration = LibGit2Sharp.Configuration.BuildFrom(System.IO.Path.Combine(this.RepositoryURL, ".gitmodules"));
            gitModulesConfiguration.Set<string>(@"submodule." + newRelativePath + ".url", submoduleURL);
            gitModulesConfiguration.Set<string>(@"submodule." + newRelativePath + ".path", newRelativePath);
            string gitLinkPath = System.IO.Path.Combine(newAbsolutePath, ".git");
            string gitDirFilePath = System.IO.Path.Combine(gitDirectoryPath, "gitdir");
            string submoduleConfigPath = System.IO.Path.Combine(gitDirectoryPath, "config");
            if (System.IO.File.Exists(gitLinkPath))
            {
                System.IO.File.WriteAllText(gitLinkPath, "gitdir: " + new Uri(gitLinkPath, UriKind.Absolute).MakeRelativeUri(new Uri(gitDirectoryPath, UriKind.Absolute)));
            }

            if (System.IO.File.Exists(gitDirFilePath))
            {
                System.IO.File.WriteAllText(gitDirFilePath, new Uri(this.RepositoryURL, UriKind.Absolute).MakeRelativeUri(new Uri(gitLinkPath, UriKind.Absolute)).ToString());
            }

            if (System.IO.File.Exists(submoduleConfigPath))
            {
                Configuration submoduleConfiguration = LibGit2Sharp.Configuration.BuildFrom(submoduleConfigPath);
                submoduleConfiguration.Set<string>("core.worktree", new Uri(submoduleConfigPath, UriKind.Absolute).MakeRelativeUri(new Uri(newAbsolutePath, UriKind.Absolute)).ToString());
            }

            this.StageFile(".gitmodules");
            this.StageFile(newRelativePath);
        }

        /// <summary>
        /// Moves the repository to the path specified as argument.
        /// </summary>
        /// <param name="pathToMoveTo">The new path of the repository.</param>
        public void MoveTo(string pathToMoveTo)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            // If it is a submodule, we can't move it by our own so we do nothing
            if (System.IO.File.Exists(System.IO.Path.Combine(this.RepositoryURL, ".git")))
                return;

            if (System.IO.Directory.Exists(pathToMoveTo))
                DirectoryExtensions.DeleteCompletely(pathToMoveTo);
            string repositoryURL = this.RepositoryURL;
            this.Dispose();
            System.IO.Directory.Move(repositoryURL, pathToMoveTo);
            this.gitRepositoryHandle = new Repository(pathToMoveTo);

        }

        /// <summary>
        /// Pull the new commits inside the local branch from the branch tracked in the remote repository.
        /// </summary>
        public void Pull()
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (this.gitRepositoryHandle.Head.Remote == null)
                throw new ModuniException("There is no remote branch that is tracked by the current branch.");

            this.gitRepositoryHandle.Network.Pull(this.gitRepositoryHandle.Config.BuildSignature(new DateTimeOffset()), new PullOptions(){ FetchOptions = new FetchOptions(){ TagFetchMode = TagFetchMode.All }, MergeOptions = new MergeOptions(){ FastForwardStrategy = FastForwardStrategy.Default } });
        }

        /// <summary>
        /// Push the new commits of the local branch to the branch tracked in the remote repository.
        /// </summary>
        public void Push()
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (this.gitRepositoryHandle.Head.Remote == null)
            {
                if (!this.gitRepositoryHandle.Network.Remotes.Any())
                {
                    throw new ModuniException("There is no remote where to push the commits.");
                }
                else
                {
                    Remote originRemote = this.gitRepositoryHandle.Network.Remotes.FirstOrDefault((Remote remote) => remote.Name == "origin");
                    if (originRemote == null)
                    {
                        originRemote = this.gitRepositoryHandle.Network.Remotes.First();
                    }

                    this.gitRepositoryHandle.Branches.Update(this.gitRepositoryHandle.Head, (BranchUpdater branchUpdater) => branchUpdater.Remote = originRemote.Name, (BranchUpdater branchUpdater) => branchUpdater.UpstreamBranch = this.gitRepositoryHandle.Head.CanonicalName);
                }
            }

            this.gitRepositoryHandle.Network.Push(this.gitRepositoryHandle.Head);
        }

        /// <summary>
        /// Removes the file whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to be removed.</param>
        /// <param name="removeFromWorkingDirectory">If set to <c>true</c>, it removes the file from the working directory.</param>
        public void RemoveFile(string relativePath, bool removeFromWorkingDirectory)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            this.gitRepositoryHandle.Remove(relativePath, removeFromWorkingDirectory);
        }

        /// <summary>
        /// Removes the submodule whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the submodule to be removed.</param>
        /// <param name="forceRemoval">If set to <c>true</c>, it deletes the submodule even if it has local modifications.</param>
        public void RemoveSubmodule(string relativePath, bool forceRemoval)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            if (forceRemoval)
                DirectoryExtensions.DeleteCompletely(System.IO.Path.Combine(this.RepositoryURL, relativePath));
            this.RemoveFile(relativePath, true);
            this.DeleteSubmoduleFromConfiguration(relativePath);
            this.StageFile(".gitmodules");
            DirectoryExtensions.DeleteCompletely(System.IO.Path.Combine(System.IO.Path.Combine(this.RepositoryURL, ".git/modules"), relativePath));
        }

        /// <summary>
        /// Adds all the files inside the repository that have changed since the last commit to the staging area for the
        /// next commit.
        /// </summary>
        public void StageAllChanges()
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            List<StatusEntry> allFilesToAdd = new List<StatusEntry>();
            allFilesToAdd.AddRange(this.gitRepositoryHandle.RetrieveStatus().Untracked);
            allFilesToAdd.AddRange(this.gitRepositoryHandle.RetrieveStatus().Missing);
            allFilesToAdd.AddRange(this.gitRepositoryHandle.RetrieveStatus().Modified);
            foreach (StatusEntry file in allFilesToAdd)
            {
                this.gitRepositoryHandle.Stage(file.FilePath);
            }
        }

        /// <summary>
        /// Stages the file whose relative path is specified as argument.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to be staged.</param>
        public void StageFile(string relativePath)
        {
            if (!this.IsRepositoryInitialized)
                throw new ModuniException("The repository is not initialized.");

            this.gitRepositoryHandle.Stage(relativePath);
        }

        void DeleteSubmoduleFromConfiguration(string relativePath)
        {
            Configuration gitModulesConfiguration = LibGit2Sharp.Configuration.BuildFrom(System.IO.Path.Combine(this.RepositoryURL, ".gitmodules"));
            gitModulesConfiguration.Unset(@"submodule." + relativePath + ".url");
            gitModulesConfiguration.Unset(@"submodule." + relativePath + ".path");
            gitModulesConfiguration.Unset(@"submodule." + relativePath + ".branch");
        }
    }
}

