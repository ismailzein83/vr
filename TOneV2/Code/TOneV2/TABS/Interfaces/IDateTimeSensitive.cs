using System;
using System.Collections.Generic;

namespace TABS.Interfaces
{
    /// <summary>
    /// A Marker Interface for Objects that *are* or *contain* Date time sensitive information,
    /// like the Carrier Account which has Supply Effective Rates and Sale Effective Rates, that may change 
    /// with time (and date)
    /// </summary>
    public interface IDateTimeSensitive
    {
        IDateTimeSensitive RefreshableContainer { get; }
        IEnumerable<IDateTimeSensitive> RefreshableChildren { get; }
        DateTime? LastRefresh { get; }
        void Refresh();
    }
}
