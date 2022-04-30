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
