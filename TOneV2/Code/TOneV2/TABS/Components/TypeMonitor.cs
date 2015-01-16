using System;
using System.Collections.Generic;

namespace TABS.Components
{
    public class TypeMonitor : Interfaces.IDateTimeSensitive
    {
        Type _MonitoredType;

        public TypeMonitor(Type typeToMonitor)
        {
            _MonitoredType = typeToMonitor;
        }

        #region IDateTimeSensitive Members

        public TABS.Interfaces.IDateTimeSensitive RefreshableContainer
        {
            get { return null; }
        }

        public IEnumerable<TABS.Interfaces.IDateTimeSensitive> RefreshableChildren
        {
            get { return null; }
        }

        protected DateTime? _LastRefresh;

        public DateTime? LastRefresh
        {
            get { return _LastRefresh; }
        }

        public void Refresh()
        {
            ObjectAssembler.ClearCachedCollections(_MonitoredType);
            _LastRefresh = DateTime.Now;
        }

        #endregion
    }
}