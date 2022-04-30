// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

using System;

namespace TimeLog.Models
{
    public class TimeLogEntry
    {
        public Int64 Id { get; set; }
        public string TicketId { get; set; }
        public string Comment { get; set; }
        public DateTime? TimeReported { get; set; }
        public TimeSpan? Duration { get; set; } 
        public bool Flushed { get; set; }
        public bool SkipFlushing { get; set; }

        public TimeLogEntry()
        {
            Flushed = false;
            SkipFlushing = false;
            TimeReported = null;
            Duration = null;
        }
    }
}
