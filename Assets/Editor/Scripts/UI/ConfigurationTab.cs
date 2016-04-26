//
//  ConfigurationTab.cs
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
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Moduni.UI
{
    public class ConfigurationTab : ITab, IMessageTrigger
    {
        public delegate void OnConfigurationSavedHandler(DeveloperSettings developerSettings,IList<ARepositoryManagerSettings> repositoriesManagerSettings,bool isProjectOnlySettings);

        public event OnConfigurationSavedHandler OnConfigurationSaved;

        private DeveloperSettings developerSettings;
        private DeveloperSettings originalDeveloperSettings;
        private List<ARepositoryManagerSettings> repositoriesManagersSettings;
        private IEnumerable<ARepositoryManagerSettings> originalRepositoriesManagersSettings;
        private RepositoryManagerSettingsFactory repositoryManagerSettingsFactory;
        private Vector2 scrollPositionRepositoriesManagersList;
        private List<string> settingsTypes;
        private GUIStyles styles;

        public ConfigurationTab(DeveloperSettings developerSettings, IEnumerable<ARepositoryManagerSettings> repositoriesManagersSettings, GUIStyles styles)
        {
            this.originalDeveloperSettings = developerSettings;
            this.originalRepositoriesManagersSettings = repositoriesManagersSettings;

            this.CloneOriginalSettings();

            this.repositoryManagerSettingsFactory = new RepositoryManagerSettingsFactory();
            this.settingsTypes = new List<string>(this.repositoryManagerSettingsFactory.GetSettingsTypes());
            this.styles = styles;
        }

        private void CloneOriginalSettings()
        {
            this.developerSettings = (DeveloperSettings)this.originalDeveloperSettings.Clone();
            this.repositoriesManagersSettings = new List<ARepositoryManagerSettings>();
            foreach (ARepositoryManagerSettings repositoryManagerSettings in this.originalRepositoriesManagersSettings)
            {
                this.repositoriesManagersSettings.Add((ARepositoryManagerSettings)repositoryManagerSettings.Clone());
            }
        }

        private void SaveConfiguration(bool isProjectOnlySettings)
        {
            this.originalDeveloperSettings = this.developerSettings;
            this.originalRepositoriesManagersSettings = this.repositoriesManagersSettings;
            List<ARepositoryManagerSettings> clonedRepositoriesManagersSettings = new List<ARepositoryManagerSettings>();
            foreach (ARepositoryManagerSettings repositoryManagerSettings in this.repositoriesManagersSettings)
            {
                clonedRepositoriesManagersSettings.Add((ARepositoryManagerSettings)repositoryManagerSettings.Clone());
            }
            if (OnConfigurationSaved != null)
                OnConfigurationSaved((DeveloperSettings)this.developerSettings.Clone(), clonedRepositoriesManagersSettings, isProjectOnlySettings);
            if (OnMessageTriggered != null)
                OnMessageTriggered(new Message("Configuration saved !", MessageType.Info));
        }

        #region IMessageTrigger implementation

        public event OnMessageTriggeredHandler OnMessageTriggered;

        #endregion

        #region IGUIElement implementation

        public void Display()
        {
            AEditor<ARepositoryManagerSettings> repositoryManagerSettingsEditor;
            int previousSelectedTypeIndex, selectedTypeIndex;

            using (EditorGUILayout.HorizontalScope horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                    GUILayout.Space(50f);
                }
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(1000f)))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    // Developer settings
                    EditorGUILayout.LabelField(new GUIContent("Developer settings"), this.styles.BoldLabelStyle, null);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    this.developerSettings.fullName = EditorGUILayout.TextField("Name", this.developerSettings.fullName, EditorStyles.textField, null);
                    this.developerSettings.emailAddress = EditorGUILayout.TextField("Email", this.developerSettings.emailAddress, EditorStyles.textField, null);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    // Repositories settings
                    EditorGUILayout.LabelField(new GUIContent("Repositories settings"), this.styles.BoldLabelStyle, null);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(this.scrollPositionRepositoriesManagersList))
                    {
                        this.scrollPositionRepositoriesManagersList = scrollView.scrollPosition;

                        if (this.repositoriesManagersSettings.Count == 0)
                        {
                            EditorGUILayout.LabelField("No repositories found. You have to add a repository before using the modules manager.", this.styles.BoldLabelStyle, null);
                        }
                            
                        if (GUILayout.Button("Add new repository manager", GUILayout.MaxWidth(250f), GUILayout.MinHeight(50f)))
                        {
                            this.repositoriesManagersSettings.Add(this.repositoryManagerSettingsFactory.CreateSettings(this.repositoryManagerSettingsFactory.GetSettingsTypes()[0]));
                        }

                        EditorGUILayout.Space();
                        EditorGUILayout.Space();
                        for (int i = this.repositoriesManagersSettings.Count - 1; i >= 0; i--)
                        {
                            using (EditorGUILayout.HorizontalScope horizontalScope2 = new EditorGUILayout.HorizontalScope())
                            {
                                using (EditorGUILayout.VerticalScope verticalScope2 = new EditorGUILayout.VerticalScope())
                                {
                                    previousSelectedTypeIndex = this.settingsTypes.FindIndex((string settingsTypeName) => this.repositoryManagerSettingsFactory.GetSettingsTypeName(this.repositoriesManagersSettings[i]) == settingsTypeName);
                                    selectedTypeIndex = EditorGUILayout.Popup(previousSelectedTypeIndex, this.settingsTypes.ToArray(), GUILayout.MaxWidth(500f));
                                    if (previousSelectedTypeIndex != selectedTypeIndex)
                                    {
                                        this.repositoriesManagersSettings[i] = this.repositoryManagerSettingsFactory.CreateSettings(this.settingsTypes[selectedTypeIndex]);
                                    }
                                    EditorGUILayout.Space();

                                    repositoryManagerSettingsEditor = this.repositoryManagerSettingsFactory.GetEditor(this.repositoriesManagersSettings[i]);
                                    repositoryManagerSettingsEditor.Display();

                                    EditorGUILayout.Space();
                                    EditorGUILayout.Space();
                                }

                                if (GUILayout.Button("Remove", GUILayout.MinHeight(75f)))
                                {
                                    this.repositoriesManagersSettings.RemoveAt(i);
                                }
                            }
                        }

                    }
                    using (EditorGUILayout.HorizontalScope horizontalScope2 = new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                        }
                        if (GUILayout.Button("Save configuration for project", GUILayout.MinHeight(50f), GUILayout.MaxWidth(250f)))
                        {
                            this.SaveConfiguration(true);
                        }
                        if (GUILayout.Button("Save configuration for project and editor", GUILayout.MinHeight(50f), GUILayout.MaxWidth(250f)))
                        {
                            this.SaveConfiguration(false);
                        }
                        // Reset all the settings to their saved states before any modification of the user.
                        if (GUILayout.Button("Reset configuration", GUILayout.MinHeight(50f), GUILayout.MaxWidth(250f)))
                        {
                            this.CloneOriginalSettings();
                            if (OnMessageTriggered != null)
                                OnMessageTriggered(new Message("Configuration reset !", MessageType.Info));
                        }
                        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                        {
                        }
                    }
                }
                using (EditorGUILayout.VerticalScope verticalScope = new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                    GUILayout.Space(50f);
                }
            }
        }

        #endregion

        #region ITab implementation

        public string Name
        {
            get
            {
                return "Configuration";
            }
        }

        public void Reset()
        {
            this.CloneOriginalSettings();
        }

        #endregion
    }
}

