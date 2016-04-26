//
//  Message.cs
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
using System.Globalization;

namespace Moduni
{
    public struct Message
    {
        public DateTimeOffset dateTime;
        public string content;
        public MessageType type;

        public Message(string content, MessageType type)
        {
            this.dateTime = DateTimeOffset.Now;
            this.content = content;
            this.type = type;
        }

        public string Format()
        {
            return string.Format("[{0}] {1}", this.dateTime.LocalDateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture), this.content);
        }
    }
}

