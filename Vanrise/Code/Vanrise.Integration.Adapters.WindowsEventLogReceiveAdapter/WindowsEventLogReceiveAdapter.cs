using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Vanrise.Common;
using Vanrise.Integration.Entities;
using System.Collections.Generic;
using Vanrise.Integration.Business;
using System.Security;
using System.Diagnostics.Eventing.Reader;
using Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments;

namespace Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter
{
    public class WindowsEventLogReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            WindowsEventLogAdapterArgument windowsEventLogAdapterArgument = context.AdapterArgument as WindowsEventLogAdapterArgument;
            windowsEventLogAdapterArgument.ThrowIfNull("windowsEventLogAdapterArgument");
            windowsEventLogAdapterArgument.HostName.ThrowIfNull("HostName");
            windowsEventLogAdapterArgument.Domain.ThrowIfNull("Domain");
            windowsEventLogAdapterArgument.UserName.ThrowIfNull("UserName");
            windowsEventLogAdapterArgument.Password.ThrowIfNull("Password");
            windowsEventLogAdapterArgument.Sources.ThrowIfNull("Sources");
            if (windowsEventLogAdapterArgument.Sources.Count == 0)
                throw new Exception("At least one source should be selected.");

            WindowsEventLogAdapterState windowsEventLogAdapterState = SaveOrGetAdapterState(context, windowsEventLogAdapterArgument);

            SecureString securedPassword = new SecureString();
            foreach (var ch in windowsEventLogAdapterArgument.Password)
            {
                securedPassword.AppendChar(ch);
            }
            securedPassword.MakeReadOnly();
            EventLogSession eventLogSession = new EventLogSession(windowsEventLogAdapterArgument.HostName, windowsEventLogAdapterArgument.Domain, windowsEventLogAdapterArgument.UserName, securedPassword, SessionAuthentication.Default);
            foreach (var source in windowsEventLogAdapterArgument.Sources)
            {
                var sourceState = windowsEventLogAdapterState.EventStateBySource.GetOrCreateItem(source);
               
                var startTime = sourceState.LastEventTime;
                var endTime = DateTime.Now;
                long lastId = sourceState.LastEventId;

                var querySelect = string.Format("*[System[TimeCreated[@SystemTime >= '{0}']]] and *[System[TimeCreated[@SystemTime <= '{1}']]] and *[System[EventID > '{2}']]", startTime.ToString("o"), endTime.ToString("o"), lastId);
                var eventLogQuery = new EventLogQuery("Application", PathType.LogName, querySelect);
                eventLogQuery.Session = eventLogSession;

                EventLogReader reader = new EventLogReader(eventLogQuery);

                var count = 0;

                List<WindowsEventLog> newEvents = new List<WindowsEventLog>();

                while (count <= reader.BatchSize)
                {
                    var readerdata = reader.ReadEvent();
                    if (readerdata != null)
                    {
                        string message = readerdata.FormatDescription();
                        if(readerdata.RecordId.HasValue)
                          lastId = readerdata.RecordId.Value;
                        newEvents.Add(new WindowsEventLog
                        {
                            TimeCreated = readerdata.TimeCreated,
                            Description = readerdata.FormatDescription()
                        });
                    }
                   
                    count++;
                }

                if (newEvents.Count > 0)
                {
                    context.OnDataReceived(new WidowsEventLogImportedData
                    {
                        Events = newEvents
                    });
                }

                sourceState.LastEventTime = endTime;
                sourceState.LastEventId = lastId;
            }

            eventLogSession.Dispose();

            windowsEventLogAdapterState = SaveOrGetAdapterState(context, windowsEventLogAdapterArgument, windowsEventLogAdapterState.EventStateBySource);

            base.LogVerbose("Establishing SFTP Connection");
        }

        #region Private Functions

        private WindowsEventLogAdapterState SaveOrGetAdapterState(IAdapterImportDataContext context, WindowsEventLogAdapterArgument windowsEventLogAdapterArgument, Dictionary<string, WindowsEventLogAdapterSourceState> eventStateBySource = null)
        {
            WindowsEventLogAdapterState adapterState = null;
            context.GetStateWithLock((state) =>
            {
                adapterState = state as WindowsEventLogAdapterState;

                if (adapterState == null)
                {
                    adapterState = new WindowsEventLogAdapterState
                    {
                       EventStateBySource = new Dictionary<string, WindowsEventLogAdapterSourceState>()
                    };
                }
                if (eventStateBySource != null)
                {
                    adapterState.EventStateBySource = eventStateBySource;
                }
                return adapterState;
            });

            return adapterState;
        }
        #endregion

    }
}
