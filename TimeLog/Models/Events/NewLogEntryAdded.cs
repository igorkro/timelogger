using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Events
{
    class NewLogEntryAdded: TLEvent
    {
        public TimeLogEntry Entry { get; set; }

        public NewLogEntryAdded(TimeLogEntry entry)
        {
            Entry = entry;
        }
    }
}
