//
//  ModuleState.cs
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
using System.Xml;
using UnityEngine;
using System.IO;

namespace Moduni
{
    public class ModuleState : IXmlSerializable, ICloneable
    {
        private string name;
        private string description;
        private int trl;
        private string path;
        private List<Tag> tags;
        private List<Dependency> dependencies;

        public string Name
        { 
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string Description
        { 
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public int TRL
        { 
            get
            {
                return this.trl;
            }
            set
            {
                this.trl = Math.Min(Math.Max(value, 1), 9);
            }
        }

        public string Path
        { 
            get
            {
                if (this.path != null)
                    return new Uri(System.IO.Path.Combine(Application.dataPath, this.path)).AbsolutePath;
                else
                    return null;
            }
            set
            {
                value = value.Replace('\\', '/');
                if (value.StartsWith(Application.dataPath))
                    this.path = value.Remove(0, Application.dataPath.Length + 1);
                else
                    this.path = value;
            }
        }

        public IList<Tag> Tags
        { 
            get
            {
                return this.tags;
            }
        }

        public IList<Dependency> Dependencies
        { 
            get
            {
                return this.dependencies;
            }
        }

        public ModuleState()
        {
            this.name = "My module";
            this.description = "Write your description here...";
            this.trl = 1;
            this.path = null;
            this.tags = new List<Tag>();
            this.dependencies = new List<Dependency>();
        }

        public object Clone()
        {
            ModuleState clone = new ModuleState();
            clone.dependencies = new List<Dependency>();
            foreach (Dependency dependency in this.dependencies)
            {
                clone.dependencies.Add((Dependency)dependency.Clone());
            }
            clone.description = this.description;
            clone.name = this.name;
            clone.path = this.path;
            clone.tags = new List<Tag>();
            foreach (Tag tag in this.tags)
            {
                clone.tags.Add(tag);
            }
            clone.trl = this.trl;
            return clone;

        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this.name = reader.ReadElementString("Name");
            this.path = reader.ReadElementString("Path");
            this.description = reader.ReadElementString("Description");
            reader.ReadStartElement("TRL");
            this.trl = reader.ReadContentAsInt();
            reader.ReadEndElement();
            XmlSerializer tagSerializer = new XmlSerializer(typeof(List<Tag>), new XmlRootAttribute("Tags"));
            this.tags = (List<Tag>)tagSerializer.Deserialize(reader);
            XmlSerializer dependencySerializer = new XmlSerializer(typeof(List<Dependency>), new XmlRootAttribute("Dependencies"));
            this.dependencies = (List<Dependency>)dependencySerializer.Deserialize(reader);
        }

        /// <summary>
        /// Determines whether the state is valid for a module.
        /// </summary>
        /// <returns>Returns an empty string if it is valid or a message with the errors found during the validation.</returns>
        public string Validate(bool isCreation)
        {
            string result = string.Empty;
            if (this.path == null)
            {
                result += "The path to the repository should not be empty." + Environment.NewLine;
            }
            else
            {
                if (!this.path.EndsWith(".git"))
                {
                    result += "The folder should end with the suffix '.git'." + Environment.NewLine;
                }
                    
                if (isCreation && Directory.GetFileSystemEntries(this.Path).Length == 0)
                {
                    result += "The directory of the module should not be empty at its creation." + Environment.NewLine;
                }
            }

            return result;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Name", this.name);
            writer.WriteElementString("Path", this.path);
            writer.WriteElementString("Description", this.description);
            writer.WriteStartElement("TRL");
            writer.WriteValue(this.trl);
            writer.WriteEndElement();
            XmlSerializer tagSerializer = new XmlSerializer(typeof(List<Tag>), new XmlRootAttribute("Tags"));
            XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add("", "");
            tagSerializer.Serialize(writer, this.tags, xmlSerializerNamespaces);
            XmlSerializer dependencySerializer = new XmlSerializer(typeof(List<Dependency>), new XmlRootAttribute("Dependencies"));
            dependencySerializer.Serialize(writer, this.dependencies, xmlSerializerNamespaces);
        }
    }
}