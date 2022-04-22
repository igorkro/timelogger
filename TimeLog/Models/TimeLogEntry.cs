using System;
using System.Collections.Generic;
using System.Text;

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
