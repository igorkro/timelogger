using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using TimeLog.Models;
using TimeLog.Models.Events;
using TimeLog.Utils;
using static TimeLog.Utils.JiraRepository;

namespace TimeLog
{
    class NetworkService: IDisposable, EventHandler
    {
        Deque<NetworkJob> jobs = new Deque<NetworkJob>();
        readonly object lockObject = new object();
        readonly object credentialsLock = new object();
        ManualResetEvent resetEvent = new ManualResetEvent(false);
        bool isRunning = false;
        Thread workThread = null;
        bool isPaused = false;
        bool hadNetworkIssue = false;

        private CredentialManagement.Credential loadedCredentials = null;

        public NetworkService()
        {
        }

        public void Start()
        {
            EventBroker.Get().Subscribe(typeof(AppFinish), this);
            EventBroker.Get().Subscribe(typeof(JiraCredentialsUpdated), this);

            isRunning = true;
            resetEvent.Set();

            workThread = new Thread(workThreadFunc);
            workThread.Start();
        }

        public void Stop()
        {
            if (!isRunning)
                return;
            isRunning = false;
            resetEvent.Set();

            if (workThread != null)
            {
                workThread.Join();
            }
        }

        public void Pause()
        {
            isPaused = true;

            lock (lockObject)
            {
                Logger.Get().Log('I', string.Format("Network service paused. Outstanding jobs:{0}", jobs.Count));
            }
        }

        public void Resume()
        {
            isPaused = false;

            lock (lockObject)
            {
                Logger.Get().Log('I', string.Format("Network service resumed. Outstanding jobs:{0}", jobs.Count));
            }

            resetEvent.Set();
        }

        private void workThreadFunc()
        {
            NetworkJob currentJob = null;
            while (isRunning)
            {
                if (!isPaused)
                {
                    currentJob = null;
                    lock (lockObject)
                    {
                        if (jobs.Count > 0)
                        {
                            currentJob = jobs.RemoveFront();
                        }
                    }

                    if (currentJob != null)
                    {
                        Logger.Get().Log('I', string.Format("Processing network job. JobId:{0} JobTitle:{1}", currentJob.JobId, currentJob.JobTitle));

                        processJob(currentJob);
                        continue;
                    }
                }
                resetEvent.WaitOne();
            }
        }

        public bool IsLogFlushingQueued(long worklogId)
        {
            lock (lockObject)
            {
                foreach (NetworkJob job in jobs)
                {
                    if (!(job.UserData is JobInfo))
                        continue;

                    JobInfo jobInfo = (JobInfo)job.UserData;
                    if (jobInfo.InternalWorklogId == worklogId)
                        return true;
                }
            }
            return false;
        }

        public void PushJob(NetworkJob job)
        {
            lock (lockObject)
            {
                if (jobs.Contains(job))
                    return;

                jobs.AddBack(job);

                Logger.Get().Log('I', string.Format("New network job. JobId:{0} JobTitle:{1} Total:{2}", job.JobId, job.JobTitle, jobs.Count));
            }

            if (!isPaused)
                resetEvent.Set();
        }

        private void processJob(NetworkJob job)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    Uri ticketUrl = job.RequestUri;

                    client.BaseAddress = ticketUrl;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    string authHeader = getAuthHeader();
                    bool isAccessToken = Services.Get().Config.IsAccessTokenAuth;
                    if (authHeader != null)
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(isAccessToken ? "Bearer" : "Basic", authHeader);

                    HttpResponseMessage result = null;
                    if (job.Method == HttpMethod.Post)
                    {
                        result = client.PostAsync("", new StringContent(job.Payload, Encoding.UTF8, "application/json")).Result;
                    }
                    else
                    {
                        result = client.GetAsync("").Result;
                    }

                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        Logger.Get().Log('E', string.Format("Request unauthorized. URL:{0} Response:{1}", ticketUrl.ToString(), resultContent));

                        Pause();

                        lock (lockObject)
                        {
                            jobs.AddFront(job);
                        }

                        releaseCredentials();
                        hadNetworkIssue = true;
                        EventBroker.Get().Notify(new InvalidJiraCredentialsDetected());

                        return;
                    }

                    if ((int)result.StatusCode >= 500)
                    {
                        Logger.Get().Log('E', string.Format("Request internal error. URL:{0} StatusCode:{1}", ticketUrl.ToString(), result.StatusCode.ToString()));

                        Pause();

                        lock (lockObject)
                        {
                            jobs.AddFront(job);
                        }

                        hadNetworkIssue = true;
                        EventBroker.Get().Notify(new NetworkIssueDetected(string.Format("Failed to {0}", job.JobTitle)));
                        EventBroker.Get().Notify(new NetworkJobFinished(job, (int)result.StatusCode, string.Empty));
                        return;
                    }

                    if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Logger.Get().Log('E', string.Format("Not found. URL:{0} Response:{1}", ticketUrl.ToString(), resultContent));
                        EventBroker.Get().Notify(new NetworkJobFinished(job, 404, resultContent));
                        return;
                    }

                    if (hadNetworkIssue)
                    {
                        hadNetworkIssue = false;
                        EventBroker.Get().Notify(new NetworkIssueResolved());
                    }

                    Logger.Get().Log('I', string.Format("Request processed. URL:{0} Response:{1}", ticketUrl.ToString(), resultContent));
                    EventBroker.Get().Notify(new NetworkJobFinished(job, (int)result.StatusCode, resultContent));
                }
            }
            catch (Exception ex)
            {
                Logger.Get().Log('E', string.Format("Exception. URL:{0} Error:{1}", job.RequestUri.ToString(), ex.ToString()));
                Pause();

                lock (lockObject)
                {
                    jobs.AddFront(job);
                }

                hadNetworkIssue = true;
                EventBroker.Get().Notify(new NetworkIssueDetected(string.Format("Failed to {0}, got an exception (check logs)", job.JobTitle)));
                EventBroker.Get().Notify(new NetworkJobFinished(job, 0, string.Empty));
            }
        }

        private string getAuthHeader()
        {
            if (loadedCredentials == null)
            {
                loadCredentials();
            }

            if (Services.Get().Config.IsAccessTokenAuth)
            {
                if (loadedCredentials == null || string.IsNullOrEmpty(loadedCredentials.Password))
                    return null;
                return loadedCredentials.Password;
            }

            if (loadedCredentials == null || string.IsNullOrEmpty(loadedCredentials.Username))
                return null;
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(new StringBuilder().Append(loadedCredentials.Username).Append(":").Append(loadedCredentials.Password).ToString()));
        }

        private void releaseCredentials()
        {
            lock (credentialsLock)
            {
                if (loadedCredentials != null)
                    loadedCredentials.Dispose();
                loadedCredentials = null;
            }
        }

        private void loadCredentials()
        {
            lock (credentialsLock)
            {
                if (loadedCredentials != null)
                    return;

                loadedCredentials = Services.Get().Creds.Load();
            }
        }

        public void Dispose()
        {
            EventBroker.Get().UnsubscribeFromAll(this);
            lock (credentialsLock)
            {
                releaseCredentials();
            }
            Stop();
        }

        public void OnTLEvent(TLEvent e, ref bool stopProcessing)
        {
            if (e is JiraCredentialsUpdated)
            {
                Logger.Get().Log('I', "Credentials updated.");
                releaseCredentials();
                Resume();
            }
            else if (e is AppFinish)
            {
                Stop();
            }
        }
    }
}
