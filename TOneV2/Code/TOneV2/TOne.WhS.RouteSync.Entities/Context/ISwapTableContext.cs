using System;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public interface ISwapTableContext
    {
        string SwitchName { get; }

        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }

        int IndexesCommandTimeoutInSeconds { get; }

        string SwitchId { get; }

        SwitchSyncOutput PreviousSwitchSyncOutput { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        Action<Exception, bool> WriteBusinessHandledException { get; }
    }

    public class SwapTableContext : ISwapTableContext
    {
        public string SwitchName { get; set; }

        public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }

        public int IndexesCommandTimeoutInSeconds { get; set; }

        public string SwitchId { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }

        public SwitchSyncOutput PreviousSwitchSyncOutput { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }
    }
}