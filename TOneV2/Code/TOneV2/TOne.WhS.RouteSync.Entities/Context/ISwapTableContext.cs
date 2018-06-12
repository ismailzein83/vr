using System;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public interface ISwapTableContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }

        int IndexesCommandTimeoutInSeconds { get; }

        SwitchSyncOutput PreviousSwitchSyncOutput { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        object Payload { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }
    }

    public class SwapTableContext : ISwapTableContext
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }

        public int IndexesCommandTimeoutInSeconds { get; set; }

        public SwitchSyncOutput PreviousSwitchSyncOutput { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public object Payload { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }
}