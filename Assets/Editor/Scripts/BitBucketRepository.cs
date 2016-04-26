//
//  BitBucketRepository.cs
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
    public class BitBucketRepository : ISourceControlRepository
    {
        private string repositoryURL;

        public BitBucketRepository()
        {
            this.repositoryURL = null;
        }

        public System.Collections.Generic.IEnumerable<string> Branches
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentBranch
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string CurrentCommit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.Generic.IEnumerable<RepositoryFile> Files
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string FirstCommitReference
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsBare
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsDirty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRepositoryInitialized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.Generic.IEnumerable<string> LocalBranches
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(this.RemoteOriginURL))
                {
                    string[] splitRemoteOriginURL = this.RemoteOriginURL.Split(new string[4]{ "/", "//", "\\", "\\\\" }, StringSplitOptions.RemoveEmptyEntries);
                    return splitRemoteOriginURL[splitRemoteOriginURL.Length - 1];
                }
                else
                    return null;
            }
        }

        public string RemoteOriginURL
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string RepositoryURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISourceControlRepository AddSubmodule(string submoduleRepositoryURL, string submoduleRelativePath)
        {
            throw new NotImplementedException();
        }

        public void AddTag(string newTagName)
        {
            throw new NotImplementedException();
        }

        public void CheckoutReference(string referenceName)
        {
            throw new NotImplementedException();
        }

        public void Commit(string message)
        {
            throw new NotImplementedException();
        }

        public bool ContainsBranch(string branchName)
        {
            throw new NotImplementedException();
        }

        public void CreateBranch(string branchName)
        {
            throw new NotImplementedException();
        }

        public void DeleteWorkingCopy()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void FetchAll()
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<string> FindTags(System.Text.RegularExpressions.Regex regexToMatch)
        {
            throw new NotImplementedException();
        }

        public CommitSignature? GetCommitSignature(string commitReference)
        {
            throw new NotImplementedException();
        }

        public string GetFileContent(string relativePathToFile, BranchVersion branchVersion = default(BranchVersion))
        {
            throw new NotImplementedException();
        }

        public string GetNearestTagFromCommit(string commitReference)
        {
            throw new NotImplementedException();
        }

        public string GetSubmoduleRelativePath(string fullPath)
        {
            throw new NotImplementedException();
        }

        public ISourceControlRepository InitOrCloneRepository(string path)
        {
            throw new NotImplementedException();
        }

        public void MoveSubmodule(string oldRelativePath, string newRelativePath)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(string pathToMoveTo)
        {
            throw new NotImplementedException();
        }

        public void Pull()
        {
            throw new NotImplementedException();
        }

        public void Push()
        {
            throw new NotImplementedException();
        }

        public void RemoveFile(string relativePath, bool removeFromWorkingDirectory)
        {
            throw new NotImplementedException();
        }

        public void RemoveSubmodule(string relativePath, bool forceRemoval)
        {
            throw new NotImplementedException();
        }

        public void StageAllChanges()
        {
            throw new NotImplementedException();
        }

        public void StageFile(string relativePath)
        {
            throw new NotImplementedException();
        }
    }
}

