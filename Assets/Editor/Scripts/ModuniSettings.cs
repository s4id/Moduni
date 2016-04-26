//
//  ModuniSettings.cs
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
using System.Xml.Serialization;
using UnityEngine;
using LibGit2Sharp;
using System.Xml.Schema;
using System.Xml;
using System.Runtime.Serialization;

namespace Moduni
{
    [Serializable]
    public class ModuniSettings
    {
        private DeveloperSettings developerSettings;
        private List<ARepositoryManagerSettings> repositoryManagerSettings;

        [XmlIgnore]
        public DeveloperSettings DeveloperSettings
        {
            get
            {
                return this.developerSettings;
            }
            set
            {
                this.developerSettings = value;
            }
        }

        [XmlArrayItem("FileSystemRepositoryManagerSettings", typeof(FileSystemRepositoryManagerSettings))]
        [XmlArrayItem("SSHServerRepositoryManagerSettings", typeof(SSHServerRepositoryManagerSettings))]
        [XmlArrayItem("BitBucketRepositoryManagerSettings", typeof(BitBucketRepositoryManagerSettings))]
        public List<ARepositoryManagerSettings> RepositoryManagerSettings
        {
            get
            {
                return this.repositoryManagerSettings;
            }
            set
            {
                this.repositoryManagerSettings = value;
            }
        }

        public ModuniSettings()
        {
            this.developerSettings = new DeveloperSettings();
            Configuration gitConfiguration = LibGit2Sharp.Configuration.BuildFrom(null);
            if (gitConfiguration.HasConfig(ConfigurationLevel.Global) || gitConfiguration.HasConfig(ConfigurationLevel.System))
            {
                this.developerSettings.fullName = gitConfiguration.Get<string>("user.name").Value;
                this.developerSettings.emailAddress = gitConfiguration.Get<string>("user.email").Value;
            }

            this.repositoryManagerSettings = new List<ARepositoryManagerSettings>();
        }
    }
}

