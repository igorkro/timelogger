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
