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

using CredentialManagement;

namespace TimeLog.Models
{
    class CredentialRepository
    {
        static readonly string CredentialTarget = "IK_TimeLogger";

        public Credential Load()
        {
            Credential credential = new Credential();
            credential.Target = CredentialTarget;
            if (credential.Load())
                return credential;

            return null;
        }


        public void Save(string username, string password)
        {
            using (var cred = new Credential())
            {
                cred.Target = CredentialTarget;
                cred.Load();

                if (username != null)
                    cred.Username = username;

                if (password != null)
                    cred.Password = password;

                cred.Type = CredentialType.Generic;
                cred.PersistanceType = PersistanceType.LocalComputer;
                cred.Save();
            }
        }
    }
}
