//
//  ModuleTests.cs
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
using System.Xml;
using System.Xml.Serialization;

namespace Moduni.Tests
{
    [TestFixture]
    public class ModuleTests
    {
        private Module module;
        private string pathToModuleXml;

        public ModuleTests()
        {
            this.pathToModuleXml = Path.Combine(GlobalTestConstants.TestDataPath, "ModuleSerialization.xml");
        }

        [SetUp]
        public void Init()
        {
            this.module = new Module();
            this.module.Name = "Test";
            this.module.Path = Path.Combine(Application.dataPath, "Test.git");
            this.module.Description = "Description test";
            this.module.TRL = 2;
            this.module.Tags.Add(new Tag("TagTest", Color.red));
        }

        [Test, Description("Serialize a module into XML successfully.")]
        public void WriteXml_Successfully()
        {
            // Setup the test
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.WriteWhitespace(" ");

            // Run the test
            xmlTextWriter.WriteStartElement("Module");
            this.module.WriteXml(xmlTextWriter);
            xmlTextWriter.WriteEndElement();

            File.WriteAllText(this.pathToModuleXml, stringWriter.ToString());

            // Validate the test
            Assert.AreEqual(File.ReadAllText(this.pathToModuleXml), stringWriter.ToString());
        }

        [Test, Description("Deserialize a module into an object successfully.")]
        public void ReadXml_Successfully()
        {
            // Setup the test
            StringReader stringReader = new StringReader(File.ReadAllText(this.pathToModuleXml));
            XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
            Module moduleDeserialized = new Module();

            // Run the test
            moduleDeserialized.ReadXml(xmlTextReader);

            // Validate the test
            Assert.AreEqual(this.module.Name, moduleDeserialized.Name);
            Assert.AreEqual(this.module.Path, moduleDeserialized.Path);
            Assert.AreEqual(this.module.Description, moduleDeserialized.Description);
            Assert.AreEqual(this.module.TRL, moduleDeserialized.TRL);
            Assert.AreEqual(this.module.Tags[0], moduleDeserialized.Tags[0]);
        }

        [TearDown]
        public void Teardown()
        {
        }
    }
}
