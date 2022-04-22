namespace TimeLog.Models.Events
{
    class NetworkJobFinished: TLEvent
    {
        public NetworkJob Job { get; private set; }
        public int StatusCode { get; private set; }
        public string Payload { get; private set; }

        public NetworkJobFinished(NetworkJob job, int statusCode, string payload)
        {
            Job = job;
            StatusCode = statusCode;
            Payload = payload;
        }
    }
}
