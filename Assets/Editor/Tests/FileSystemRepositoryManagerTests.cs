//
//  FileSystemRepositoryManagerTests.cs
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
using UnityEditor;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Moduni.Tests
{
    [TestFixture]
    public class FileSystemRepositoryManagerTests
    {
        private FileSystemRepositoryManager fileSystemRepositoryManager;
        private FileSystemRepositoryManagerSettings fileSystemRepositoryManagerSettings;

        [SetUp]
        public void Init()
        {
            this.fileSystemRepositoryManager = new FileSystemRepositoryManager();
            this.fileSystemRepositoryManagerSettings = new FileSystemRepositoryManagerSettings();
            this.fileSystemRepositoryManagerSettings.folderPath = Path.Combine(GlobalTestConstants.TemporaryTestDataPath, "Repositories");
            this.fileSystemRepositoryManager.Settings = this.fileSystemRepositoryManagerSettings;
            Directory.CreateDirectory(this.fileSystemRepositoryManagerSettings.folderPath);
        }

        [Test, Description("Creates a new repository with a valid input name 'TestModule' and a valid result.")]
        public void CreateRepository_Successful()
        {
            // Setup the test
            const string testModuleName = "TestModule";

            // Run the test
            Task<ISourceControlRepository> requestResult = this.fileSystemRepositoryManager.CreateRepository(testModuleName);
            requestResult.Wait();

            // Validate the test
            Assert.That(requestResult.Status == TaskStatus.RanToCompletion);
            Assert.IsNotNull(requestResult.Result);
            Assert.AreEqual(testModuleName + ".git", requestResult.Result.Name);
            Assert.That(requestResult.Result.IsRepositoryInitialized);
            Assert.That(File.Exists(Path.Combine(this.fileSystemRepositoryManagerSettings.folderPath, testModuleName + ".git") + Path.DirectorySeparatorChar + "config"));
            Assert.That(File.Exists(Path.Combine(this.fileSystemRepositoryManagerSettings.folderPath, testModuleName + ".git") + Path.DirectorySeparatorChar + "HEAD"));
        }

        [Test, Description("Throw an ArgumentException because it is creating a repository with an empty name.")]
        public void CreateRepository_EmptyArgument()
        {
            // Run and validate the test
            Task<ISourceControlRepository> requestResult = this.fileSystemRepositoryManager.CreateRepository(string.Empty);
            try
            {
                requestResult.Wait();
            }
            catch (AggregateException aggregateException)
            {
                Assert.That(aggregateException.InnerException.GetType() == typeof(ArgumentException) && aggregateException.InnerException.Message.Contains("empty"));
            }
        }

        [Test, Description("Throw an ArgumentException because it is creating a repository with an invalid name.")]
        public void CreateRepository_InvalidName()
        {
            // Setup the test
            string moduleName = "Test";
            foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
            {
                moduleName += invalidCharacter;
            }

            // Run and validate the test
            Task<ISourceControlRepository> requestResult = this.fileSystemRepositoryManager.CreateRepository(moduleName);
            try
            {
                requestResult.Wait();
            }
            catch (AggregateException aggregateException)
            {
                Assert.That(aggregateException.InnerException.GetType() == typeof(ArgumentException) && aggregateException.InnerException.Message.Contains("Illegal characters"));
            }
        }

        [TearDown]
        public void Teardown()
        {
            DirectoryExtensions.DeleteCompletely(GlobalTestConstants.TemporaryTestDataPath);
        }
    }
}
