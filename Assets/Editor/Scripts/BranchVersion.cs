//
//  BranchVersion.cs
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
using System.Text.RegularExpressions;

namespace Moduni
{
    public struct BranchVersion : IComparable<BranchVersion>
    {
        public static Regex RegexBranchVersion = new Regex(@"^v([0-9]*\.){2}x*$");
        public static Regex RegexVersion = new Regex(@"^v([0-9]*\.){2}[0-9]*$");

        public uint? major;
        public uint? minor;
        public uint? patch;
        public string branch;

        public BranchVersion(uint major, uint minor, uint patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
            this.branch = null;
        }

        public BranchVersion(string branchVersion)
        {
            this.major = null;
            this.minor = null;
            this.patch = null;
            this.branch = null;

            Match matchVersion = BranchVersion.RegexVersion.Match(branchVersion);
            if (matchVersion.Success)
            {
                string matchVersionValue = matchVersion.Value;
                matchVersionValue = matchVersionValue.Remove(0, 1);
                string[] versionComponents = matchVersionValue.Split('.');
                this.major = uint.Parse(versionComponents[0]);
                this.minor = uint.Parse(versionComponents[1]);
                this.patch = uint.Parse(versionComponents[2]);
                return;
            }

            this.branch = branchVersion;

            Match matchBranchVersion = BranchVersion.RegexBranchVersion.Match(branchVersion);
            if (matchBranchVersion.Success)
            {
                string matchBranchVersionValue = matchBranchVersion.Value;
                matchBranchVersionValue = matchBranchVersionValue.Remove(0, 1);
                string[] versionComponents = matchBranchVersionValue.Split('.');
                this.major = uint.Parse(versionComponents[0]);
                this.minor = uint.Parse(versionComponents[1]);
            }
        }

        public void IncreaseMajorVersion()
        {
            if (this.IsVersion)
            {
                this.major = this.major.Value + 1;
                this.minor = 0;
                this.patch = 0;
            }
        }

        public void IncreaseMinorVersion()
        {
            if (this.IsVersion)
            {
                this.minor = this.minor.Value + 1;
                this.patch = 0;
            }
        }

        public void IncreasePatchVersion()
        {
            if (this.IsVersion)
            {
                this.patch = this.patch.Value + 1;
            }
        }

        public override string ToString()
        {
            if (this.IsVersion)
                return string.Format("v{0}.{1}.{2}", this.major, this.minor, this.patch);
            else
                return this.branch;
        }

        public string ToBranchString()
        {
            if (!this.IsBranch)
                return string.Format("v{0}.{1}.x", this.major, this.minor);
            return this.branch;
        }

        public bool IsVersion
        {
            get
            {
                return this.patch.HasValue;
            }
        }

        public bool IsBranchVersion
        {
            get
            {
                return !string.IsNullOrEmpty(this.branch) && this.major.HasValue;
            }
        }

        public bool IsBranch
        {
            get
            {
                return !string.IsNullOrEmpty(this.branch) && !this.major.HasValue;
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BranchVersion branchVersion1, BranchVersion branchVersion2)
        {
            return branchVersion1.CompareTo(branchVersion2) == 0;
        }

        public static bool operator !=(BranchVersion branchVersion1, BranchVersion branchVersion2)
        {
            return !(branchVersion1 == branchVersion2);
        }

        public static bool operator >=(BranchVersion branchVersion1, BranchVersion branchVersion2)
        {
            return branchVersion1.CompareTo(branchVersion2) >= 0;
        }

        public static bool operator <=(BranchVersion branchVersion1, BranchVersion branchVersion2)
        {
            return branchVersion1.CompareTo(branchVersion2) <= 0;
        }

        #region IComparable implementation

        public int CompareTo(BranchVersion other)
        {
            if ((this.IsVersion || this.IsBranchVersion) && (other.IsVersion || other.IsBranchVersion))
            {
                if (this.major > other.major)
                {
                    return 1;
                }
                else if (other.major > this.major)
                {
                    return -1;
                }
                else
                {
                    if (this.minor > other.minor)
                    {
                        return 1;
                    }
                    else if (other.minor > this.minor)
                    {
                        return -1;
                    }
                    else
                    {
                        if (this.IsVersion && other.IsVersion)
                        {
                            if (this.patch > other.patch)
                            {
                                return 1;
                            }
                            else if (other.patch > this.patch)
                            {
                                return -1;
                            }
                        }
                        else if (this.IsBranchVersion && other.IsVersion)
                        {
                            return 1;
                        }
                        else if (this.IsVersion && other.IsBranchVersion)
                        {
                            return -1;
                        }
                        return 0;
                    }
                }
            }
            else if (this.IsBranch && !other.IsBranch)
            {
                return 1;
            }
            else if (!this.IsBranch && other.IsBranch)
            {
                return -1;
            }
            else
            {
                if ((this.branch == "development" || this.branch == "master") && other.branch != "development" && other.branch != "master")
                {
                    return 1;
                }
                else if ((other.branch == "development" || other.branch == "master") && this.branch != "development" && this.branch != "master")
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion
    }
}

