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
            using (EventLogSession eventLogSession = new EventLogSession(windowsEventLogAdapterArgument.HostName, windowsEventLogAdapterArgument.Domain, windowsEventLogAdapterArgument.UserName, securedPassword, SessionAuthentication.Default))
            {
                foreach (var source in windowsEventLogAdapterArgument.Sources)
                {
                    var sourceState = windowsEventLogAdapterState.EventStateBySource.GetOrCreateItem(source);

                    var startTime = sourceState.LastEventTime;
                    if (startTime == DateTime.MinValue && windowsEventLogAdapterArgument.InitialStartDate.HasValue)
                        startTime = windowsEventLogAdapterArgument.InitialStartDate.Value;
                    long lastId = sourceState.LastEventId;
                    DateTime maxDate = sourceState.LastEventTime;

                    var querySelect = string.Format("*[System[TimeCreated[@SystemTime >= '{0}']]] and *[System[EventID > '{1}']]", startTime.ToString("o"), lastId);
                    var eventLogQuery = new EventLogQuery(source, PathType.LogName, querySelect);
                    eventLogQuery.Session = eventLogSession;
                    int batchSize = windowsEventLogAdapterArgument.BatchSize;
                    using (EventLogReader reader = new EventLogReader(eventLogQuery))
                    {
                        List<WindowsEventLog> newEvents = new List<WindowsEventLog>();

                        EventRecord readerdata = null;
                        do
                        {
                            readerdata = reader.ReadEvent();
                            if (readerdata != null)
                            {
                                DateTime? timeCreated = null;
                                string description = null;
                                string xml = null;
                                string machineName = null;
                                string levelDisplayName = null;
                                string taskDisplayName = null;
                                string providerName = null;

                                try
                                {
                                    timeCreated = readerdata.TimeCreated;
                                    if (timeCreated.HasValue)
                                        maxDate = timeCreated.Value;
                                }
                                catch (Exception ex)
                                {
                                    base.LogError("Cannot read property 'TimeCreated', Reason:'{0}'", ex.Message);
                                }
                                try
                                {
                                    if (readerdata.RecordId.HasValue)
                                        lastId = readerdata.RecordId.Value;
                                }
                                catch (Exception ex)
                                {
                                    base.LogError("Cannot read property 'RecordId', Reason:'{0}'",  ex.Message);
                                }

                                try
                                {
                                    if (readerdata.UserId != null)
                                    {
                                        description = readerdata.FormatDescription();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    base.LogError("Cannot read property 'FormatDescription', Reason:'{0}'", ex.Message);
                                }

                                try
                                {
                                    xml = readerdata.ToXml();
                                }
                                catch (Exception ex)
                                {
                                    base.LogError("Cannot read property 'ToXml', Reason:'{0}'", ex.Message);
                                }


                                try
                                {
                                    machineName = readerdata.MachineName;
                                    levelDisplayName = readerdata.LevelDisplayName;
                                    taskDisplayName = readerdata.TaskDisplayName;
                                    providerName = readerdata.ProviderName;
                                }
                                catch(Exception ex)
                                {
                                    base.LogError("Cannot read properties from event logs, Reason:'{0}'", ex.Message);
                                }

                                newEvents.Add(new WindowsEventLog
                                {
                                    TimeCreated = timeCreated,
                                    Description = description,
                                    MachineName = machineName,
                                    LevelDisplayName = levelDisplayName,
                                    TaskDisplayName= taskDisplayName,
                                    ProviderName = providerName,
                                    DescriptionXml = xml
                                });
                            }

                            batchSize--;
                            if(batchSize == 0)
                            {
                                if (newEvents.Count > 0)
                                {
                                    context.OnDataReceived(new WidowsEventLogImportedData
                                    {
                                        Events = newEvents
                                    });
                                    windowsEventLogAdapterState = SaveOrGetAdapterState(context, windowsEventLogAdapterArgument, windowsEventLogAdapterState.EventStateBySource);
                                }
                                batchSize = windowsEventLogAdapterArgument.BatchSize;
                            }
                            sourceState.LastEventTime = maxDate;
                            sourceState.LastEventId = lastId;
                        }
                        while (readerdata != null);
                        if (newEvents.Count > 0)
                        {
                            context.OnDataReceived(new WidowsEventLogImportedData
                            {
                                Events = newEvents
                            });
                            windowsEventLogAdapterState = SaveOrGetAdapterState(context, windowsEventLogAdapterArgument, windowsEventLogAdapterState.EventStateBySource);
                        }
                    }
                }
            }            
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
