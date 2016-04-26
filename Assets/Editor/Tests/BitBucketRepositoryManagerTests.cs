//
//  BitBucketRepositoryManagerTests.cs
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
using NSubstitute;
using Moduni;
using System.Threading.Tasks;
using UnityEngine.Experimental.Networking;
using System.Collections.Generic;
using System.Collections;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Deserializers;
using System;

namespace Moduni.Tests
{
    [TestFixture]
    public class BitBucketRepositoryManagerTests
    {
        private BitBucketRepositoryManager bitBucketRepositoryManager;
        private BitBucketRepositoryManagerSettings bitBucketRepositoryManagerSettings;
        private IRestClient restClient;

        [SetUp]
        public void Init()
        {
            this.bitBucketRepositoryManager = new BitBucketRepositoryManager();
            this.bitBucketRepositoryManagerSettings = new BitBucketRepositoryManagerSettings(){ projectKey = "TSTPRJ" };
            this.bitBucketRepositoryManager.Settings = this.bitBucketRepositoryManagerSettings;
            this.restClient = Substitute.For<IRestClient>();
            this.bitBucketRepositoryManager.RestClient = this.restClient;
        }

        [Test, Description("Creates a new repository with a valid input name 'TestModule' and a valid result.")]
        public void CreateRepository_Successful()
        {
            // Setup the test
            string testModuleName = "TestModule";
            IRestResponse<BitBucketRepository> restResponse = Substitute.For<IRestResponse<BitBucketRepository>>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            BitBucketRepository bitBucketRepository = new BitBucketRepository();
            restResponse.Data.Returns(bitBucketRepository);
            this.restClient.ExecuteAsPost<BitBucketRepository>(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos" && request.Method == Method.POST), Arg.Is<string>("POST")).Returns(restResponse);

            // Run the test
            Task<ISourceControlRepository> requestResult = this.bitBucketRepositoryManager.CreateRepository(testModuleName);
            requestResult.Wait();

            // Validate the test
            Assert.That(requestResult.Status == TaskStatus.RanToCompletion);
            Assert.IsNotNull(requestResult.Result);
            Assert.AreEqual(testModuleName + ".git", requestResult.Result.Name);
        }

        [Test, Description("Try to create a new repository with a valid input name 'TestModule' but fail because of a bad request error.")]
        public void CreateRepository_BadRequestError()
        {
            // Setup the test
            string testModuleName = "TestModule";
            IRestResponse<GitRepository> restResponse = Substitute.For<IRestResponse<GitRepository>>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Error);
            restResponse.StatusCode.Returns(System.Net.HttpStatusCode.BadRequest);
            this.restClient.ExecuteAsPost<GitRepository>(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos" && request.Method == Method.POST), Arg.Is<string>("POST")).Returns(restResponse);

            // Run the test
            Task<ISourceControlRepository> requestResult = this.bitBucketRepositoryManager.CreateRepository(testModuleName);

            // Validate the test
            Assert.Throws<AggregateException>(() => requestResult.Wait(), "Awaiting the task should throw an exception due to the bad request error.");
            Assert.That(requestResult.Status == TaskStatus.Faulted);
            Assert.That(requestResult.Exception.InnerException.GetType() == typeof(Exception));
            Assert.That(requestResult.Exception.InnerException.Message.Contains(System.Net.HttpStatusCode.BadRequest.ToString()));
        }

        [Test, Description("Lists successfully all the repositories of the Bit Bucket repository manager.")]
        public void GetRepositories_Successful()
        {
            // Setup the test
            IRestResponse<List<GitRepository>> restResponse = Substitute.For<IRestResponse<List<GitRepository>>>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            List<GitRepository> repositories = new List<GitRepository>();
            string firstRepositoryName = "Test1.git", secondRepositoryName = "Test2.git", thirdRepositoryName = "Test3.git";
            repositories.Add(new GitRepository(){ RemoteOriginURL = "ssh://bitbucket.domain.com/" + firstRepositoryName });
            repositories.Add(new GitRepository(){ RemoteOriginURL = "ssh://bitbucket.domain.com/" + secondRepositoryName });
            repositories.Add(new GitRepository(){ RemoteOriginURL = "ssh://bitbucket.domain.com/" + thirdRepositoryName });
            restResponse.Data.Returns(repositories);
            this.restClient.ExecuteAsGet<List<GitRepository>>(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos" && request.Method == Method.GET), Arg.Is<string>("GET")).Returns(restResponse);

            // Run the test
            Task<IEnumerable<ISourceControlRepository>> requestResult = this.bitBucketRepositoryManager.GetRepositories();
            requestResult.Wait();

            // Validate the test
            Assert.That(requestResult.Status == TaskStatus.RanToCompletion);
            Assert.IsNotNull(requestResult.Result);
            IEnumerator<ISourceControlRepository> repositoriesIterator = requestResult.Result.GetEnumerator();
            repositoriesIterator.MoveNext();
            Assert.AreEqual(firstRepositoryName, repositoriesIterator.Current.Name);
            repositoriesIterator.MoveNext();
            Assert.AreEqual(secondRepositoryName, repositoriesIterator.Current.Name);
            repositoriesIterator.MoveNext();
            Assert.AreEqual(thirdRepositoryName, repositoriesIterator.Current.Name);
        }

        [Test, Description("Fail to list the repositories of the Bit Bucket repository manager because of a bad request error.")]
        public void GetRepositories_BadRequestError()
        {
            // Setup the test
            IRestResponse<List<GitRepository>> restResponse = Substitute.For<IRestResponse<List<GitRepository>>>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Error);
            restResponse.StatusCode.Returns(System.Net.HttpStatusCode.BadRequest);
            this.restClient.ExecuteAsGet<List<GitRepository>>(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos" && request.Method == Method.GET), Arg.Is<string>("GET")).Returns(restResponse);

            // Run the test
            Task<IEnumerable<ISourceControlRepository>> requestResult = this.bitBucketRepositoryManager.GetRepositories();

            // Validate the test
            Assert.Throws<AggregateException>(() => requestResult.Wait(), "Awaiting the task should throw an exception due to the bad request error.");
            Assert.That(requestResult.Status == TaskStatus.Faulted);
            Assert.That(requestResult.Exception.InnerException.GetType() == typeof(Exception));
            Assert.That(requestResult.Exception.InnerException.Message.Contains(System.Net.HttpStatusCode.BadRequest.ToString()));
        }

        [Test, Description("Delete successfully a repository.")]
        public void DeleteRepository_Successful()
        {
            // Setup the test
            GitRepository repositoryToBeDeleted = new GitRepository(){ RemoteOriginURL = "ssh://bitbucket.domain.com/TestModule.git" };
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            this.restClient.Execute(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos/" + repositoryToBeDeleted.Name && request.Method == Method.DELETE)).Returns(restResponse);

            // Run the test
            Task requestResult = this.bitBucketRepositoryManager.DeleteRepository(repositoryToBeDeleted);
            requestResult.Wait();

            // Validate the test
            Assert.That(requestResult.Status == TaskStatus.RanToCompletion);
        }

        [Test, Description("Fail to delete the repository whose name is 'TestModule' because of a bad request error.")]
        public void DeleteRepository_BadRequestError()
        {
            // Setup the test
            GitRepository repositoryToBeDeleted = new GitRepository(){ RemoteOriginURL = "ssh://bitbucket.domain.com/@/{" };
            IRestResponse restResponse = Substitute.For<IRestResponse>();
            restResponse.ResponseStatus.Returns(ResponseStatus.Error);
            restResponse.StatusCode.Returns(System.Net.HttpStatusCode.BadRequest);
            this.restClient.Execute(Arg.Is((IRestRequest request) => request.Resource == "/rest/api/1.0/projects/" + this.bitBucketRepositoryManagerSettings.projectKey + "/repos/" + repositoryToBeDeleted.Name && request.Method == Method.DELETE)).Returns(restResponse);

            // Run the test
            Task requestResult = this.bitBucketRepositoryManager.DeleteRepository(repositoryToBeDeleted);

            // Validate the test
            Assert.Throws<AggregateException>(() => requestResult.Wait(), "Awaiting the task should throw an exception due to the bad request error.");
            Assert.That(requestResult.Status == TaskStatus.Faulted);
            Assert.That(requestResult.Exception.InnerException.GetType() == typeof(Exception));
            Assert.That(requestResult.Exception.InnerException.Message.Contains(System.Net.HttpStatusCode.BadRequest.ToString()));
        }
    }
}