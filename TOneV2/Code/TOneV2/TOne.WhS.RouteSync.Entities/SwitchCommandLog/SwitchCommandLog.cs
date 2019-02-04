using System;

namespace TOne.WhS.RouteSync.Entities
{
    public class SwitchCommandLog
    {
        public long Id { get; set; }

        public long ProcessInstanceId { get; set; }

        public string SwitchId { get; set; }

        public string Command { get; set; }

        public string Response { get; set; }
    }
}