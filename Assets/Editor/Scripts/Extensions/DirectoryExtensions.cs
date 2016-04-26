//
//  DirectoryExtensions.cs
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
namespace Moduni
{
    using System;
    using System.IO;
    using System.Threading;

    public static class DirectoryExtensions
    {
        public static void Copy(string sourceDirectoryName, string destinationDirectoryName, bool copySubDirectories)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourceDirectoryName);

            if (!sourceDirectory.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirectoryName);
            }

            DirectoryInfo[] subDirectories = sourceDirectory.GetDirectories();
            if (!Directory.Exists(destinationDirectoryName))
            {
                Directory.CreateDirectory(destinationDirectoryName);
            }

            FileInfo[] files = sourceDirectory.GetFiles();
            foreach (FileInfo file in files)
            {
                string newFilePath = Path.Combine(destinationDirectoryName, file.Name);
                file.CopyTo(newFilePath, false);
            }

            if (copySubDirectories)
            {
                foreach (DirectoryInfo subDirectory in subDirectories)
                {
                    string newDirectoryPath = Path.Combine(destinationDirectoryName, subDirectory.Name);
                    DirectoryExtensions.Copy(subDirectory.FullName, newDirectoryPath, copySubDirectories);
                }
            }
        }

        public static void DeleteCompletely(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
                int countDeletionTries = 0;
                foreach (string file in files)
                {
                    countDeletionTries = 0;
                    while (countDeletionTries >= 0 && countDeletionTries < 100)
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        try
                        {
                            if (File.Exists(file))
                                File.Delete(file);
                            countDeletionTries = -1;
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(0);
                            ++countDeletionTries;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Thread.Sleep(0);
                            ++countDeletionTries;
                        }
                    }
                }
                countDeletionTries = 0;
                while (countDeletionTries >= 0 && countDeletionTries < 100)
                {
                    try
                    {
                        if (Directory.Exists(directoryPath))
                        {
                            Directory.Delete(directoryPath, true);
                        }
                        countDeletionTries = -1;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(0);
                        ++countDeletionTries;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Thread.Sleep(0);
                        ++countDeletionTries;
                    }
                }
            }
        }
    }
}