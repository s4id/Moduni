//
//  BitBucketRepositoryManager.cs
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
using RestSharp;
using UnityEngine;
using System.Linq;
using RestSharp.Authenticators;

namespace Moduni
{
    public class BitBucketRepositoryManager : IRepositoryManager
    {
        [Serializable]
        public struct GetRepository
        {
            public int size { get; set; }

            public int limit { get; set; }

            public bool isLastPage { get; set; }

            public int start { get; set; }
        }

        internal struct PostRepository
        {
            internal string name;
            internal string scmId;
            internal bool forkable;
        }

        private IRestClient restClient;
        private BitBucketRepositoryManagerSettings settings;

        internal IRestClient RestClient
        {
            set
            {
                this.restClient = value;
            }
        }

        internal BitBucketRepositoryManager()
        {
        }

        #region IRepositoryManager implementation

        public Color Color
        {
            get
            {
                return this.settings.color;
            }
        }

        public string Name
        {
            get
            {
                return this.settings.name;
            }
        }

        public ARepositoryManagerSettings Settings
        {
            set
            {
                this.settings = (BitBucketRepositoryManagerSettings)value;
                this.restClient = new RestSharp.RestClient(string.Format("{0}://{1}:{2}", this.settings.scheme, this.settings.host, this.settings.Port));
                this.restClient.Authenticator = new HttpBasicAuthenticator(this.settings.username, this.settings.DecryptedPassword);
            }
        }

        /// <summary>
        /// Creates a new repository whose name is specified in the argument.
        /// </summary>
        /// <returns>The new repository created.</returns>
        /// <param name="name">The name of the new repository.</param>
        public Task<ISourceControlRepository> CreateRepository(string name)
        {
            return Task.Factory.StartNew<ISourceControlRepository>(() =>
                {
                    IRestRequest restRequest = new RestRequest("/rest/api/1.0/projects/" + this.settings.projectKey + "/repos", Method.POST);
                    PostRepository requestBody = new PostRepository(){ name = name, scmId = "git", forkable = true };
                    restRequest.AddJsonBody(requestBody);
                    IRestResponse<BitBucketRepository> restResponse = this.restClient.ExecuteAsPost<BitBucketRepository>(restRequest, "POST");
                    switch (restResponse.ResponseStatus)
                    {
                        case ResponseStatus.Completed:
                            return restResponse.Data;
                        case ResponseStatus.Error:
                            throw new Exception(restResponse.StatusCode.ToString() + ": " + restResponse.ErrorMessage);
                    }
                    return null;
                });
        }

        public Task DeleteRepository(ISourceControlRepository repositoryToDelete)
        {
            return Task.Factory.StartNew(() =>
                {
                    IRestRequest restRequest = new RestRequest("/rest/api/1.0/projects/" + this.settings.projectKey + "/repos/" + repositoryToDelete.Name, Method.DELETE);
                    IRestResponse restResponse = this.restClient.Execute(restRequest);
                    switch (restResponse.ResponseStatus)
                    {
                        case ResponseStatus.Completed:
                            return;
                        case ResponseStatus.Error:
                            throw new Exception(restResponse.StatusCode.ToString() + ": " + restResponse.ErrorMessage);
                    }
                    return;
                });
        }

        public Task<IEnumerable<ISourceControlRepository>> GetRepositories()
        {
            return Task.Factory.StartNew<IEnumerable<ISourceControlRepository>>(() =>
                {
                    IRestRequest restRequest = new RestRequest("/rest/api/1.0/projects/" + this.settings.projectKey + "/repos", Method.GET);
                    IRestResponse<GetRepository> restResponse = this.restClient.ExecuteAsGet<GetRepository>(restRequest, "GET");
                    Debug.Log(restResponse.Data.size);
                    Debug.Log(restResponse.Data.limit);
                    Debug.Log(restResponse.Content);
                    Debug.Log(new RestSharp.Deserializers.JsonDeserializer().Deserialize<GetRepository>(restResponse).limit);
                    switch (restResponse.ResponseStatus)
                    {
                        case ResponseStatus.Completed:
                            return new List<BitBucketRepository>().Cast<ISourceControlRepository>().ToList();
                        case ResponseStatus.Error:
                            throw new ModuniException(restResponse.StatusCode.ToString() + ": " + restResponse.ErrorMessage);
                    }
                    return null;
                });
        }

        public Task<IEnumerable<string>> GetFileTextContentFromRepositories(string relativePathInRepository)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

