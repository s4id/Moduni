//
//  Encryptor.cs
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
using System.Security.Cryptography;
using UnityEditor;
using System.IO;

namespace Moduni
{
    public class Encryptor
    {
        private const string CryptoPrefsKey = "Moduni_CryptoPrefsKey";
        private const string InitializationVectorPrefsKey = "Moduni_InitializationVectorPrefsKey";

        private RijndaelManaged rijndaelManaged;

        public Encryptor()
        {
            this.rijndaelManaged = new RijndaelManaged();
            byte[] cryptoKey;
            byte[] initializationVector;

            if (EditorPrefs.HasKey(Encryptor.CryptoPrefsKey) && EditorPrefs.HasKey(Encryptor.InitializationVectorPrefsKey))
            {
                this.rijndaelManaged.Key = Convert.FromBase64String(EditorPrefs.GetString(Encryptor.CryptoPrefsKey));
                this.rijndaelManaged.IV = Convert.FromBase64String(EditorPrefs.GetString(Encryptor.InitializationVectorPrefsKey));
            }
            else
            {
                this.rijndaelManaged.GenerateKey();
                this.rijndaelManaged.GenerateIV();

                EditorPrefs.SetString(Encryptor.CryptoPrefsKey, Convert.ToBase64String(this.rijndaelManaged.Key));
                EditorPrefs.SetString(Encryptor.InitializationVectorPrefsKey, Convert.ToBase64String(this.rijndaelManaged.IV));
            }
        }

        public string EncryptString(string stringToBeEncrypted)
        {
            using (ICryptoTransform encrypter = this.rijndaelManaged.CreateEncryptor(this.rijndaelManaged.Key, this.rijndaelManaged.IV))
            using (MemoryStream cipherStream = new MemoryStream())
            using (CryptoStream cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
            {
                cryptoStream.Write(System.Text.Encoding.UTF8.GetBytes(stringToBeEncrypted), 0, System.Text.Encoding.UTF8.GetByteCount(stringToBeEncrypted));
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(cipherStream.ToArray());
            }
        }

        public string DecryptString(string stringToBeDecrypted)
        {
            byte[] cipherText = Convert.FromBase64String(stringToBeDecrypted);
            using (ICryptoTransform decrypter = this.rijndaelManaged.CreateDecryptor(this.rijndaelManaged.Key, this.rijndaelManaged.IV))
            using (MemoryStream cipherStream = new MemoryStream(cipherText))
            using (CryptoStream cryptoStream = new CryptoStream(cipherStream, decrypter, CryptoStreamMode.Read))
            using (StreamReader streamReader = new StreamReader(cryptoStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}

