//
//  GitRepositoryTests.cs
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

namespace Moduni.Tests
{
    using NUnit.Framework;
    using System.IO;
    using System;
    using LibGit2Sharp;
    using System.Linq;

    [TestFixture]
    public class GitRepositoryTests
    {
        private string repositoryPath;
        private string repositoryOriginPath;
        private string repositoryBarePath;
        private string nameTestFile;
        private string nameTestFile2;
        private string nameTestFile3;
        private Signature testSignature;
        private GitRepository gitRepository;
        Repository gitRepositoryOriginHandle;

        [SetUp]
        public void Init()
        {
            this.testSignature = new Signature("Test", "test@testers.com", new DateTimeOffset());
            this.nameTestFile = "Test.txt";
            this.nameTestFile2 = "Test2.txt";
            this.nameTestFile3 = "Test3.txt";
            this.repositoryPath = Path.Combine(GlobalTestConstants.TemporaryTestDataPath, "TestRepository.git") + Path.DirectorySeparatorChar;
            this.repositoryOriginPath = Path.Combine(GlobalTestConstants.TemporaryTestDataPath, "RemoteTestRepository.git") + Path.DirectorySeparatorChar;
            this.repositoryBarePath = Path.Combine(GlobalTestConstants.TemporaryTestDataPath, "BareTestRepository.git") + Path.DirectorySeparatorChar;

            this.gitRepository = new GitRepository();
            this.gitRepositoryOriginHandle = new Repository(Repository.Init(this.repositoryOriginPath));
            File.WriteAllText(this.repositoryOriginPath + this.nameTestFile, "Test");
        }

        [Test, Description("Create successfully an empty git repository.")]
        public void CreateWorkingCopy_SuccessfullyEmpty()
        {
            // Run the test
            this.gitRepository.InitOrCloneRepository(this.repositoryPath);

            // Validate the test
            Assert.That(gitRepository.IsRepositoryInitialized);
            Assert.That(Directory.Exists(this.repositoryPath));
            Assert.AreEqual(new DirectoryInfo(this.repositoryPath).FullName, gitRepository.RepositoryURL);
            Assert.That(new DirectoryInfo(this.repositoryPath).GetDirectories().Length == 1);
            Assert.That(new DirectoryInfo(this.repositoryPath).GetFiles().Length == 0);
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "config"));
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "HEAD"));
        }

        [Test, Description("Create a working copy by successfully cloning a repository.")]
        public void CreateWorkingCopy_SuccessfullyClone()
        {
            // Setup the test
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile);
            gitRepositoryOriginHandle.Commit("Initial commit", testSignature, testSignature);
            gitRepository.RemoteOriginURL = this.repositoryOriginPath;

            // Run the test
            gitRepository.InitOrCloneRepository(this.repositoryPath);

            // Validate the test
            Assert.That(gitRepository.IsRepositoryInitialized);
            Assert.That(Directory.Exists(this.repositoryPath));
            Assert.AreEqual(new DirectoryInfo(this.repositoryPath).FullName, gitRepository.RepositoryURL);
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "config"));
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "HEAD"));
            Assert.That(File.Exists(this.repositoryPath + this.nameTestFile));
        }

        [Test, Description("Create a working copy by successfully cloning a repository inside a directory that is not empty.")]
        public void CreateWorkingCopy_SuccessfullyCloneNotEmpty()
        {
            // Setup the test
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile);
            gitRepositoryOriginHandle.Commit("Initial commit", testSignature, testSignature);
            gitRepository.RemoteOriginURL = this.repositoryOriginPath;
            Directory.CreateDirectory(this.repositoryPath);
            File.WriteAllText(this.repositoryPath + this.nameTestFile2, "Test");

            // Run the test
            gitRepository.InitOrCloneRepository(this.repositoryPath);

            // Validate the test
            Assert.That(gitRepository.IsRepositoryInitialized);
            Assert.AreEqual(new DirectoryInfo(this.repositoryPath).FullName, gitRepository.RepositoryURL);
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "config"));
            Assert.That(File.Exists(Path.Combine(this.repositoryPath, ".git") + Path.DirectorySeparatorChar + "HEAD"));
            Assert.That(File.Exists(this.repositoryPath + this.nameTestFile));
        }

        [Test, Description("Throw an ArgumentException because the path is empty.")]
        public void CreateWorkingCopy_EmptyPath()
        {
            // Run and validate the test
            Assert.Throws(typeof(ArgumentException), () => gitRepository.InitOrCloneRepository(string.Empty));
        }

        [Test, Description("Throw a ModuniException because the path is invalid.")]
        public void CreateWorkingCopy_InvalidPath()
        {
            // Run and validate the test
            Assert.Throws(typeof(ModuniException), () => gitRepository.InitOrCloneRepository("////"));
        }

        [Test, Description("Stage all changes successfully.")]
        public void StageAllChanges_Successfully()
        {
            // Setup the test
            Repository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository = new GitRepository(this.repositoryPath);
            File.WriteAllText(this.repositoryPath + this.nameTestFile, "Test");
            File.WriteAllText(this.repositoryPath + this.nameTestFile2, "Test");
            File.WriteAllText(this.repositoryPath + this.nameTestFile3, "Test");

            // Run the test
            gitRepository.StageAllChanges();

            // Validate the test
            RepositoryStatus repositoryStatus = gitRepositoryHandle.RetrieveStatus();
            Assert.IsTrue(repositoryStatus[this.nameTestFile].State == FileStatus.NewInIndex);
            Assert.IsTrue(repositoryStatus[this.nameTestFile2].State == FileStatus.NewInIndex);
            Assert.IsTrue(repositoryStatus[this.nameTestFile3].State == FileStatus.NewInIndex);

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when fetching because the working copy is not initialized.")]
        public void StageAllChanges_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.StageAllChanges());
        }

        [Test, Description("Fetch all the remote repositories successfully.")]
        public void FetchAll_Successfully()
        {
            // Setup the test
            gitRepository.RemoteOriginURL = this.repositoryOriginPath;
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile);
            gitRepositoryOriginHandle.Commit("Initial commit", testSignature, testSignature);
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Clone(this.repositoryOriginPath, this.repositoryPath));
            gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryOriginPath + this.nameTestFile2, "Test");
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile2);
            gitRepositoryOriginHandle.Commit("Second commit", testSignature, testSignature);

            // Run the test
            this.gitRepository.FetchAll();

            // Validate the test
            Assert.That(gitRepositoryHandle.Branches.FirstOrDefault((Branch branch) => branch.IsRemote && branch.FriendlyName == "origin/master").Tip != gitRepositoryHandle.Head.Tip);

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when fetching because the working copy is not initialized.")]
        public void FetchAll_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.FetchAll());
        }

        [Test, Description("Pull new commits successfully.")]
        public void Pull_Successfully()
        {
            // Setup the test
            gitRepository.RemoteOriginURL = this.repositoryOriginPath;
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile);
            gitRepositoryOriginHandle.Commit("Initial commit", testSignature, testSignature);
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Clone(this.repositoryOriginPath, this.repositoryPath));
            gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryOriginPath + this.nameTestFile2, "Test");
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile2);
            gitRepositoryOriginHandle.Commit("Second commit", testSignature, testSignature);

            // Run the test
            this.gitRepository.Pull();

            // Validate the test
            Assert.That(gitRepositoryHandle.Branches.FirstOrDefault((Branch branch) => branch.IsRemote && branch.FriendlyName == "origin/master").Tip == gitRepositoryOriginHandle.Head.Tip);

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when pulling because there is no remote branch tracked by the current branch.")]
        public void Pull_NoRemoteBranchTracked()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            gitRepository.GitRepositoryHandle = gitRepositoryHandle;

            // Run and validate the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Pull());

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when pulling because the working copy is not initialized.")]
        public void Pull_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Pull());
        }

        [Test, Description("Push new commits successfully.")]
        public void Push_Successfully()
        {
            // Setup the test
            gitRepositoryOriginHandle.Index.Add(this.nameTestFile);
            gitRepositoryOriginHandle.Commit("Initial commit", testSignature, testSignature);
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Clone(this.repositoryOriginPath, this.repositoryPath));
            gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryPath + this.nameTestFile2, "Test");
            gitRepositoryHandle.Index.Add(this.nameTestFile2);
            gitRepositoryHandle.Commit("Second commit", testSignature, testSignature);
            Repository gitBareRepositoryHandle = new Repository(Repository.Clone(this.repositoryOriginPath, this.repositoryBarePath, new CloneOptions(){ IsBare = true }));
            gitRepository.RemoteOriginURL = this.repositoryBarePath;


            // Run the test
            this.gitRepository.Push();

            // Validate the test
            Assert.That(gitRepositoryHandle.Branches.FirstOrDefault((Branch branch) => branch.IsRemote && branch.FriendlyName == "origin/master").Tip == gitBareRepositoryHandle.Head.Tip);

            // Cleanup the test
            gitRepositoryHandle.Dispose();
            gitBareRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when pushing because there is no remote branch tracked by the current branch.")]
        public void Push_NoRemoteBranchTracked()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            gitRepository.GitRepositoryHandle = gitRepositoryHandle;

            // Run and validate the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Push());

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when pushing because the working copy is not initialized.")]
        public void Push_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Push());
        }

        [Test, Description("Create a new commit successfully.")]
        public void Commit_Successfully()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryPath + this.nameTestFile, "Test");
            gitRepositoryHandle.Index.Add(this.nameTestFile);

            // Run the test
            this.gitRepository.Commit("Commit test");

            // Validate the test
            Assert.That(gitRepositoryHandle.Commits.Count() == 1);

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Create an empty commit that throws a ModuniException.")]
        public void Commit_Empty()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository.GitRepositoryHandle = gitRepositoryHandle;

            // Run and validate the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Commit("Commit test"));

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when committing because the working copy is not initialized.")]
        public void Commit_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.Commit("Commit test"));
        }

        [Test, Description("Switch branch from 'master' to 'develop' successfully.")]
        public void SwitchBranch_Successfully()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryPath + this.nameTestFile, "Test");
            gitRepositoryHandle.Index.Add(this.nameTestFile);
            gitRepositoryHandle.Commit("Test", this.testSignature, this.testSignature);
            gitRepositoryHandle.CreateBranch("develop");

            // Run the test
            this.gitRepository.CheckoutReference("develop");

            // Validate the test
            Assert.That(gitRepositoryHandle.Head.FriendlyName == "develop");

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw an ArgumentException when switching branch because the name of the branch to switch to is empty.")]
        public void SwitchBranch_EmptyName()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository.GitRepositoryHandle = gitRepositoryHandle;

            // Run and validate the test
            Assert.Throws(typeof(ArgumentException), () => this.gitRepository.CheckoutReference(""));

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [Test, Description("Throw a ModuniException when switching branch because the working copy is not initialized.")]
        public void SwitchBranch_WorkingCopyNotInitialized()
        {
            // Run the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.CheckoutReference("develop"));
        }

        [Test, Description("Throw a ModuniException when switching branch because the working copy is not initialized.")]
        public void SwitchBranch_BranchDoesNotExist()
        {
            // Setup the test
            LibGit2Sharp.IRepository gitRepositoryHandle = new Repository(Repository.Init(this.repositoryPath));
            this.gitRepository.GitRepositoryHandle = gitRepositoryHandle;
            File.WriteAllText(this.repositoryPath + this.nameTestFile, "Test");
            gitRepositoryHandle.Index.Add(this.nameTestFile);
            gitRepositoryHandle.Commit("Test", this.testSignature, this.testSignature);

            // Run and validate the test
            Assert.Throws(typeof(ModuniException), () => this.gitRepository.CheckoutReference("develop"));

            // Cleanup the test
            gitRepositoryHandle.Dispose();
        }

        [TearDown]
        public void Teardown()
        {
            this.gitRepository.Dispose();
            this.gitRepositoryOriginHandle.Dispose();

            DirectoryExtensions.DeleteCompletely(GlobalTestConstants.TemporaryTestDataPath);
        }
    }
}
