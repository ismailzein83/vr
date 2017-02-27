using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        public BPInstance InstanceInfo { get; set; }

        Dictionary<Type, Guid> cacheManagerIds = new Dictionary<Type, Guid>();
        public T GetCacheManager<T>() where T : class, ICacheManager
        {
            Type t = typeof(T);
            Guid cacheManagerId;
            if (!cacheManagerIds.TryGetValue(t, out cacheManagerId))
            {
                lock (cacheManagerIds)
                {
                    if (!cacheManagerIds.TryGetValue(t, out cacheManagerId))
                    {
                        cacheManagerId = Guid.NewGuid();
                        cacheManagerIds.Add(t, cacheManagerId);
                    }
                }
            }
            return CacheManagerFactory.GetCacheManager<T>(cacheManagerId);
        }

        internal void ClearCacheManagers()
        {
            foreach (var cacheManagerId in cacheManagerIds.Values)
                CacheManagerFactory.RemoveCacheManager(cacheManagerId);
            cacheManagerIds.Clear();
        }

        public void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(false, severity, messageFormat, args);
        }

        public void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(true, severity, messageFormat, args);
        }

        public void WriteHandledException(Exception ex)
        {
            WriteHandledExceptionMessages(false, ex);
        }

        public void WriteBusinessHandledException(Exception ex)
        {
            WriteHandledExceptionMessages(true, ex);
        }

        void WriteHandledExceptionMessages(bool writeBusinessTracking, Exception ex)
        {
            BPTrackingMessage trackingMessage = new BPTrackingMessage
            {
                TrackingMessage = Utilities.GetExceptionBusinessMessage(ex),
                ExceptionDetail = ex.ToString(),
                EventTime = DateTime.Now,
                ProcessInstanceId = this.InstanceInfo.ProcessInstanceID,
                ParentProcessId = this.InstanceInfo.ParentProcessID,
                Severity = LogEntryType.Warning
            };
            BPTrackingChannel.Current.WriteTrackingMessage(trackingMessage);
            if (writeBusinessTracking)
                LoggerFactory.GetExceptionLogger().WriteException(this.InstanceInfo.InputArgument.GetDefinitionTitle(), ex);
        }

        void WriteTrackingMessages(bool writeBusinessTracking, LogEntryType severity, string messageFormat, params object[] args)
        {
            BPTrackingMessage trackingMessage = new BPTrackingMessage
            {
                TrackingMessage = args != null ? String.Format(messageFormat, args) : messageFormat,
                EventTime = DateTime.Now,
                ProcessInstanceId = this.InstanceInfo.ProcessInstanceID,
                ParentProcessId = this.InstanceInfo.ParentProcessID,
                Severity = severity
            };
            BPTrackingChannel.Current.WriteTrackingMessage(trackingMessage);
            if (writeBusinessTracking)
                LoggerFactory.GetLogger().WriteEntry(this.InstanceInfo.InputArgument.GetDefinitionTitle(), severity, messageFormat, args);
        }
    }
}
