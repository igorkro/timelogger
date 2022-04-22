using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TimeLog.Models
{
    class NetworkJob
    {
        public string JobTitle { get; private set; }
        public string JobId { get; private set; }
        public Uri RequestUri { get; private set; }
        public HttpMethod Method { get; private set; }
        public string Payload { get; private set; }
        public object UserData { get; private set; }

        public NetworkJob(string jobTitle, string jobId, Uri uri, HttpMethod method, string payload, object userData)
        {
            JobTitle = jobTitle;
            JobId = jobId;
            RequestUri = uri;
            Method = method;
            Payload = payload;
            UserData = userData;
        }
    }
}
