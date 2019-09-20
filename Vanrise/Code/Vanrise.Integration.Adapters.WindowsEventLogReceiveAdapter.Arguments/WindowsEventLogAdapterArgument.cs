using System;
using System.Collections.Generic;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments
{
    public class WindowsEventLogAdapterArgument : BaseAdapterArgument
    {
        #region Properties
        public List<string> Sources { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public string Domain { get; set; }
        public int BatchSize { get; set; }
        public DateTime? InitialStartDate { get; set; }

        #endregion
    }

    public class WindowsEventLogAdapterState : BaseAdapterState
    {
        public Dictionary<string, WindowsEventLogAdapterSourceState> EventStateBySource { get; set; }

    }
    public class WindowsEventLogAdapterSourceState
    {
        public DateTime LastEventTime { get; set; }
        public long LastEventId { get; set; }
    }
}
