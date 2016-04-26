//
//  SettingsManager.cs
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
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Moduni
{
    public class SettingsManager
    {
        const string ModuniSettingsKey = "Moduni_Settings";
        const string ModuniSettingsFilename = ".ModuniSettings.xml";

        static string ModuniSettingsPath
        {
            get
            {
                return Path.Combine(Application.dataPath, SettingsManager.ModuniSettingsFilename);
            }
        }

        private DeveloperSettings developerSettings;
        private ModuniSettings moduniSettings;
        private XmlSerializer serializer;

        public SettingsManager()
        {
            this.serializer = new XmlSerializer(typeof(ModuniSettings), "Moduni");
        }

        string GetXmlSettings()
        {
            StringWriter stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, this.moduniSettings);
            return stringWriter.ToString();
        }

        public void Load()
        {
            string moduniSettingsXml = null;
            if (File.Exists(SettingsManager.ModuniSettingsPath))
            {
                moduniSettingsXml = File.ReadAllText(SettingsManager.ModuniSettingsPath);
            }
            else if (EditorPrefs.HasKey(SettingsManager.ModuniSettingsKey))
            {
                moduniSettingsXml = EditorPrefs.GetString(SettingsManager.ModuniSettingsKey);
            }

            if (!string.IsNullOrEmpty(moduniSettingsXml))
            {
                try
                {
                    StringReader stringReader = new StringReader(moduniSettingsXml);
                    this.moduniSettings = (ModuniSettings)this.serializer.Deserialize(stringReader);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                    this.moduniSettings = new ModuniSettings();
                }
            }
            else
                this.moduniSettings = new ModuniSettings();
        }

        public void SaveForProject()
        {
            File.WriteAllText(SettingsManager.ModuniSettingsPath, this.GetXmlSettings());
        }

        public void SaveForProjectAndEditor()
        {
            string xmlSettings = this.GetXmlSettings();
            File.WriteAllText(SettingsManager.ModuniSettingsPath, xmlSettings);
            EditorPrefs.SetString(SettingsManager.ModuniSettingsKey, xmlSettings);
        }

        public ModuniSettings ModuniSettings
        {
            get
            {
                return moduniSettings;
            }
        }
    }
}

