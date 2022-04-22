using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models.Events
{
    class NetworkIssueDetected: TLEvent
    {
        public string UserFriendlyText { get; private set; }
        public Exception InnerException { get; private set; }

        public NetworkIssueDetected(string userFriendlyText, Exception innerException = null)
        {
            UserFriendlyText = userFriendlyText;
            InnerException = innerException;
        }
    }
}
