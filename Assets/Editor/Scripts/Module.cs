//
//  Module.cs
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

using System.Linq;
using UnityEditor;

namespace Moduni
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using UnityEngine;

    /// <summary>
    /// The standard module.
    /// </summary>
    [XmlRoot("Module")]
    public class Module : IModule
    {
        public static readonly string ModuleMetadataFilename = ".ModuleMetadata.xml";

        private BranchVersion currentBranchVersion;
        private ModuleState currentState;
        private FileSystemWatcher fileSystemWatcher;
        private ISourceControlRepository repository;
        private Dictionary<BranchVersion, ModuleState> states;
        private Guid uuid;
        private List<Moduni.BranchVersion> versions;

        public Module()
        {
            this.Initialize();
        }

        public Module(ISourceControlRepository sourceControlRepository)
        {
            this.repository = sourceControlRepository;
            this.Initialize();

            if (string.IsNullOrEmpty(this.repository.CurrentCommit))
                return;

            string branchVersion = this.repository.CurrentBranch;
            if (string.IsNullOrEmpty(branchVersion))
            {
                branchVersion = "master";
            }
            else if (branchVersion == "(no branch)")
            {
                branchVersion = this.repository.GetNearestTagFromCommit(this.repository.CurrentCommit);
            }

            this.CurrentBranchVersion = new BranchVersion(branchVersion);

            if (Directory.Exists(this.Path))
            {
                this.fileSystemWatcher = new FileSystemWatcher(this.Path);
                this.fileSystemWatcher.NotifyFilter = NotifyFilters.Size;
                this.fileSystemWatcher.Changed += this.OnFileSystemUpdated;
                this.fileSystemWatcher.Created += this.OnFileSystemUpdated;
                this.fileSystemWatcher.Deleted += this.OnFileSystemUpdated;
                this.fileSystemWatcher.Renamed += this.OnFileSystemUpdated;
                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Occurs when the status of the files of the module are updated.
        /// </summary>
        public event GenericEventHandler<IModule, IEnumerable<RepositoryFile>> OnFilesUpdated;
        /// <summary>
        /// Occurs when the versions of the module are updated.
        /// </summary>
        public event GenericEventHandler<IModule, IEnumerable<BranchVersion>> OnVersionsUpdated;

        /// <summary>
        /// The details about the original creator of this module.
        /// </summary>
        /// <value>The details about the original creator of this module.</value>
        public CommitSignature Creator
        {
            get
            {
                return this.repository.GetCommitSignature(this.repository.FirstCommitReference).Value;
            }
        }

        /// <summary>
        /// The current branch version that this module is using.
        /// </summary>
        /// <value>The current branch version that this module is using.</value>
        public BranchVersion CurrentBranchVersion
        {
            get
            {
                return this.currentBranchVersion;
            }

            set
            {
                if (!this.versions.Contains(value))
                {
                    throw new ModuniException("The branch version '" + value.ToString() + "' doesn't exist.");
                }
                    
                if (!value.IsVersion && !this.repository.ContainsBranch(value.ToString()))
                {
                    this.repository.CheckoutReference("origin/" + value.ToString());
                    this.repository.CreateBranch(value.ToString());
                    this.repository.CheckoutReference(value.ToString());
                }
                else
                {
                    this.repository.CheckoutReference(value.ToString());
                }

                if (!this.states.ContainsKey(value))
                {
                    this.currentState = new ModuleState();
                    this.states.Add(value, this.currentState);
                    string metadataXML = this.repository.GetFileContent(Module.ModuleMetadataFilename, value);
                    if (!string.IsNullOrEmpty(metadataXML))
                    {
                        StringReader stringReader = new StringReader(metadataXML);
                        XmlTextReader xmlReader = new XmlTextReader(stringReader);
                        this.ReadXml(xmlReader);
                    }
                }
                else
                {
                    this.currentState = this.states[value];
                }
                this.currentBranchVersion = value;
                if (this.OnFilesUpdated != null)
                    this.OnFilesUpdated(this, this.Files);
            }
        }

        /// <summary>
        /// Sets the current state of the module with the value provided.
        /// </summary>
        /// <value>The new state of the module.</value>
        public ModuleState CurrentState
        {
            set
            {
                if (this.currentState.Path != null && this.currentState.Path != value.Path)
                {
                    this.repository.MoveTo(value.Path);
                }
                this.currentState = value;
            }
        }

        /// <summary>
        /// Gets the dependencies of the module.
        /// </summary>
        /// <value>The dependencies of the module.</value>
        public IList<Dependency> Dependencies
        {
            get
            {
                return this.currentState.Dependencies;
            }
        }

        /// <summary>
        /// Gets or sets the description of the module. It should include the purpose of this module, what it is doing.
        /// </summary>
        /// <value>The description of the module, its purpose, what it is doing.</value>
        public string Description
        {
            get
            {
                return this.currentState.Description;
            }

            set
            {
                this.currentState.Description = value;
            }
        }

        public IEnumerable<RepositoryFile> Files
        {
            get
            {
                return this.repository.Files;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this module has changed since it has been imported or published.
        /// </summary>
        /// <value><c>true</c> if this module has changed since it has been imported or published; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                return this.repository.IsDirty;
            }
        }

        /// <summary>
        /// Gets the last test report of the module created by the continuous integration system.
        /// </summary>
        /// <value>The last test report of the module created by the continuous integration system.</value>
        public TestsReport LastTestReport
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the name of the file where the metadata about the module are stored.
        /// </summary>
        /// <value>The name of the file where the metadata about the module are stored.</value>
        public string MetadataFilename
        {
            get
            {
                return Module.ModuleMetadataFilename;
            }
        }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        /// <value>The name of the module.</value>
        public string Name
        {
            get
            {
                return this.currentState.Name;
            }

            set
            {
                this.currentState.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to the module inside the project.
        /// </summary>
        /// <value>The path to the module inside the project.</value>
        public string Path
        {
            get
            {
                return this.currentState.Path;
            }

            set
            {   
                this.currentState.Path = value;
            }
        }

        public string RepositoryURL
        {
            get
            {
                return this.repository.RepositoryURL;
            }
        }

        /// <summary>
        /// Gets the tags associated with this module.
        /// </summary>
        /// <value>The tags associated with this module.</value>
        public IList<Tag> Tags
        {
            get
            {
                return this.currentState.Tags;
            }
        }

        /// <summary>
        /// Gets or sets the technology readiness level of the module (a level to evaluate the maturity of a technology
        /// created by the NASA).
        /// </summary>
        /// <value>The technology readiness level of the module (a level to evaluate the maturity of a technology created by
        /// the NASA).</value>
        public int TRL
        {
            get
            {
                return this.currentState.TRL;
            }

            set
            {
                this.currentState.TRL = value;
            }
        }

        /// <summary>
        /// Gets the unique identifier of the module.
        /// </summary>
        /// <value>The unique identifier of the module.</value>
        public Guid UUID
        {
            get
            {
                return this.uuid;
            }
        }

        /// <summary>
        /// The details about the person that has created the current version of the module.
        /// </summary>
        /// <value>The details about the person that has created the current version of the module.</value>
        public CommitSignature VersionAuthor
        {
            get
            {
                return this.repository.GetCommitSignature(this.CurrentBranchVersion.ToString()).Value;
            }
        }

        /// <summary>
        /// Gets all the versions available for this module.
        /// </summary>
        /// <value>The versions available for this module.</value>
        public IEnumerable<Moduni.BranchVersion> Versions
        {
            get
            {
                return this.versions;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement("Module");
            this.uuid = new Guid(reader.ReadElementString("UUID"));
            this.currentState.ReadXml(reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("UUID", this.uuid.ToString());
            this.currentState.WriteXml(writer);
        }

        /// <summary>
        /// Browses the documentation of the module by opening it with a third party application.
        /// </summary>
        public void BrowseDocumentation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clones the current state of the module.
        /// </summary>
        /// <returns>The current state of the module.</returns>
        public ModuleState CloneState()
        {
            return (ModuleState)this.currentState.Clone();
        }

        /// <summary>
        /// Checkouts the version specified in the argument. The working copy should be in the same state as it was for
        /// this version.
        /// </summary>
        /// <param name="version">The version to checkout.</param>
        public void CheckoutBranchVersion(BranchVersion version)
        {
            this.CurrentBranchVersion = version;
        }

        /// <summary>
        /// Creates a clone of the source control repository that this module uses at the path specified in the argument.
        /// </summary>
        /// <returns>The clone of the module's repository.</returns>
        /// <param name="pathToWorkingDirectory">The path to the working directory where the repository should be cloned.</param>
        public ISourceControlRepository CloneRepository(string pathToWorkingDirectory)
        {
            ISourceControlRepository clonedRepository = this.repository.InitOrCloneRepository(pathToWorkingDirectory);
            return clonedRepository;
        }

        /// <summary>
        /// Creates the base branches that a module needs to work correctly.
        /// </summary>
        public void CreateBaseBranches()
        {
            if (this.repository.FirstCommitReference != null)
            {
                if (!this.repository.ContainsBranch("master"))
                {
                    this.repository.CreateBranch("master");
                }
            }
        }

        /// <summary>
        /// Deletes the handle of the repository.
        /// </summary>
        public void DisposeRepository()
        {
            this.repository.Dispose();
        }

        /// <summary>
        /// Publishes all the changes made to the files of the module.
        /// </summary>
        /// <param name="message">The message to explain the changes made for this publication.</param>
        public void PublishChanges(string message)
        {
            this.repository.StageAllChanges();
            this.repository.Commit(message);
            this.repository.Push();
            if (this.OnFilesUpdated != null)
                this.OnFilesUpdated(this, this.Files);
        }

        /// <summary>
        /// Publishes a new version of the module whose number is specified as argument.
        /// </summary>
        /// <param name="newVersion">The number to identify the new version.</param>
        public void PublishVersion(BranchVersion newVersion)
        {
            string branchName = newVersion.ToBranchString();
            if (!this.repository.ContainsBranch(branchName))
            {
                this.repository.CreateBranch(branchName);
                this.versions.Add(new BranchVersion(branchName));
            }

            this.repository.AddTag(newVersion.ToString());
            this.versions.Add(newVersion);
            this.SortVersionsByDescendingOrder();
        }

        /// <summary>
        /// Saves the metadata of the module.
        /// </summary>
        public void SaveMetadata()
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(new FileStream(System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, this.Path), Module.ModuleMetadataFilename), FileMode.OpenOrCreate), Encoding.UTF8){ Formatting = Formatting.Indented })
            {
                xmlWriter.WriteStartDocument(true);
                xmlWriter.WriteEndDocument();
                xmlWriter.WriteWhitespace(" ");

                // Run the test
                xmlWriter.WriteStartElement("Module");
                this.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
            }

            if (this.OnFilesUpdated != null)
                this.OnFilesUpdated(this, this.Files);
        }

        private void Initialize()
        {
            this.uuid = Guid.NewGuid();
            this.currentState = new ModuleState();
            this.states = new Dictionary<BranchVersion, ModuleState>();
            this.InitializeBranchVersions();
        }

        private void InitializeBranchVersions()
        {
            if (this.versions == null)
                this.versions = new List<BranchVersion>();
            else
                this.versions.Clear();
            this.versions.Add(new BranchVersion("master"));
            if (this.repository != null)
            {
                IEnumerable<string> versionTags = this.repository.FindTags(Moduni.BranchVersion.RegexVersion);
                BranchVersion branchVersion;
                foreach (string versionTag in versionTags)
                {
                    branchVersion = new BranchVersion(versionTag);
                    this.versions.Add(branchVersion);
                    this.versions.Add(new BranchVersion(branchVersion.ToBranchString()));
                }

                this.SortVersionsByDescendingOrder();
            }
        }

        private void OnFileSystemUpdated(object sender, FileSystemEventArgs e)
        {
            if (this.OnFilesUpdated != null)
                this.OnFilesUpdated(this, this.Files);
        }

        private void SortVersionsByDescendingOrder()
        {
            this.versions.Sort(new Comparison<BranchVersion>(
                    (branchVersion1, branchVersion2) => branchVersion2.CompareTo(branchVersion1)));
        }
    }
}