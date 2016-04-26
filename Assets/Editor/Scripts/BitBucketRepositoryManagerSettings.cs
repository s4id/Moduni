//
//  BitBucketRepositoryManagerSettings.cs
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
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Moduni
{
    [Serializable]
    public class BitBucketRepositoryManagerSettings : ARepositoryManagerSettings
    {
        public enum Scheme
        {
            http,
            https
        }

        [XmlElement("Scheme")]
        public Scheme scheme;
        [XmlElement("Host")]
        public string host;
        [XmlElement("ProjectKey")]
        public string projectKey;
        [XmlElement("Username")]
        public string username;

        private string decryptedPassword;
        private Encryptor encryptor;
        private string password;
        private int port;

        public BitBucketRepositoryManagerSettings()
        {
            this.name = "Bit Bucket";
            this.color = Color.red;
            this.host = "localhost.local";
            this.port = 443;
            this.username = "Anonymous";
            this.decryptedPassword = "password";
            this.projectKey = "PJKEY";
            this.encryptor = new Encryptor();
            this.password = this.encryptor.EncryptString(this.decryptedPassword);
        }

        [XmlIgnore]
        public string DecryptedPassword
        {
            get
            {
                if (string.IsNullOrEmpty(decryptedPassword) && !string.IsNullOrEmpty(this.password))
                {
                    this.decryptedPassword = this.encryptor.DecryptString(this.password);
                }

                return this.decryptedPassword;
            }
            set
            {
                this.decryptedPassword = value;
                if (!string.IsNullOrEmpty(this.decryptedPassword))
                {
                    this.password = this.encryptor.EncryptString(this.decryptedPassword);
                }
            }
        }

        [XmlElement("Password")]
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                if (!string.IsNullOrEmpty(this.password))
                {
                    this.decryptedPassword = this.encryptor.DecryptString(this.password);
                }
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = Convert.ToInt32(Math.Min(Math.Max(value, 0), Math.Pow(2, 16)));
            }
        }
    }
}