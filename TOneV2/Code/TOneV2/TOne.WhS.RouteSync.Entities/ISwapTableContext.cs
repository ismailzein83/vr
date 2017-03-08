using System;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public interface ISwapTableContext
    {
        string SwitchName { get; }

        Action<LogEntryType, string, object[]> WriteTrackingMessage { get; }

        int IndexesCommandTimeoutInSeconds { get; }
    }

    public class SwapTableContext : ISwapTableContext
    {
        public string SwitchName { get; set; }

        public Action<LogEntryType, string, object[]> WriteTrackingMessage { get; set; }

        public int IndexesCommandTimeoutInSeconds { get; set; }
    }
}