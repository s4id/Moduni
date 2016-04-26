//
//  RepositoryManagerSettingsFactory.cs
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
using System.Linq;
using Moduni.UI;

namespace Moduni
{
    public class RepositoryManagerSettingsFactory
    {
        private BitBucketRepositoryManagerSettingsEditor bitBucketRepositoryManagerSettingsEditor;
        private FileSystemRepositoryManagerSettingsEditor fileSystemRepositoryManagerSettingsEditor;
        private Dictionary<string,Type> settingsTypes;
        private SSHServerRepositoryManagerSettingsEditor sshServerRepositoryManagerSettingsEditor;

        public RepositoryManagerSettingsFactory()
        {
            this.bitBucketRepositoryManagerSettingsEditor = new BitBucketRepositoryManagerSettingsEditor();
            this.fileSystemRepositoryManagerSettingsEditor = new FileSystemRepositoryManagerSettingsEditor();
            this.sshServerRepositoryManagerSettingsEditor = new SSHServerRepositoryManagerSettingsEditor();
            this.settingsTypes = new Dictionary<string, Type>();
            this.settingsTypes.Add(typeof(BitBucketRepositoryManagerSettings).Name, typeof(BitBucketRepositoryManagerSettings));
            this.settingsTypes.Add(typeof(FileSystemRepositoryManagerSettings).Name, typeof(FileSystemRepositoryManagerSettings));
            this.settingsTypes.Add(typeof(SSHServerRepositoryManagerSettings).Name, typeof(SSHServerRepositoryManagerSettings));
        }

        public string[] GetSettingsTypes()
        {
            return this.settingsTypes.Keys.ToArray();
        }

        public string GetSettingsTypeName(ARepositoryManagerSettings repositoryManagerSettings)
        {
            return repositoryManagerSettings.GetType().Name;
        }

        public ARepositoryManagerSettings CreateSettings(string nameSettingsType)
        {
            Type settingsType = this.settingsTypes.FirstOrDefault((KeyValuePair<string, Type> arg) => arg.Key == nameSettingsType).Value;
            if ( settingsType == null )
                throw new NotSupportedException("This type of settings doesn't exist.");
            return (ARepositoryManagerSettings)Activator.CreateInstance(settingsType);
        }

        public AEditor<ARepositoryManagerSettings> GetEditor(ARepositoryManagerSettings repositoryManagerSettings)
        {
            if (repositoryManagerSettings is BitBucketRepositoryManagerSettings)
            {
                this.bitBucketRepositoryManagerSettingsEditor.ObjectEdited = (BitBucketRepositoryManagerSettings)repositoryManagerSettings;
                return this.bitBucketRepositoryManagerSettingsEditor;
            }
            else if (repositoryManagerSettings is FileSystemRepositoryManagerSettings)
            {
                this.fileSystemRepositoryManagerSettingsEditor.ObjectEdited = (FileSystemRepositoryManagerSettings)repositoryManagerSettings;
                return this.fileSystemRepositoryManagerSettingsEditor;
            }
            else if (repositoryManagerSettings is SSHServerRepositoryManagerSettings)
            {
                this.sshServerRepositoryManagerSettingsEditor.ObjectEdited = (SSHServerRepositoryManagerSettings)repositoryManagerSettings;
                return this.sshServerRepositoryManagerSettingsEditor;
            }

            throw new NotSupportedException("There is no editor available for this type of settings.");
        }
    }
}

