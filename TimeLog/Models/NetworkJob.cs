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
using System.Net.Http;

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
