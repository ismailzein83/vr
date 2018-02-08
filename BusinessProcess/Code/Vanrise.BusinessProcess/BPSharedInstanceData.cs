using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Persistence;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.BusinessProcess.Business;
using Vanrise.Security.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess
{
    public class BPSharedInstanceData : PersistenceParticipant
    {
        static BPDefinitionManager s_bpDefinitionManager = new BPDefinitionManager();

        BPDefinition _bpDefinition;
        string _generalLogEventType;
        int? _generalLogViewRequiredPermissionSetId;

        internal BPSharedInstanceData(BPInstance instanceInfo, BPDefinition bpDefinition)
        {
            instanceInfo.ThrowIfNull("instanceInfo");
            bpDefinition.ThrowIfNull("bpDefinition");
            this.InstanceInfo = instanceInfo;
            this._bpDefinition = bpDefinition;
            _generalLogEventType = instanceInfo.InputArgument.GetDefinitionTitle();
            _generalLogViewRequiredPermissionSetId = BPDefinitionInitiator.GetGeneralLogViewRequiredPermissionSetId(instanceInfo);
        }

        public BPInstance InstanceInfo { get; private set; }

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
            WriteTrackingMessages(false, false, severity, messageFormat, args);
        }

        public void WriteTrackingMessageToParentProcess(LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(true, true, severity, messageFormat, args);
        }

        public void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args)
        {
            WriteTrackingMessages(true, false, severity, messageFormat, args);
        }

        public void WriteHandledException(Exception ex, bool isError = false)
        {
            WriteHandledExceptionMessages(false, ex, isError);
        }

        public void WriteBusinessHandledException(Exception ex, bool isError = false)
        {
            WriteHandledExceptionMessages(true, ex, isError);
        }

        void WriteHandledExceptionMessages(bool writeBusinessTracking, Exception ex, bool isError)
        {
            BPTrackingChannel.Current.WriteException(this.InstanceInfo.ProcessInstanceID, this.InstanceInfo.ParentProcessID, ex, isError);
            if (writeBusinessTracking)
                LoggerFactory.GetExceptionLogger().WriteException(_generalLogEventType, this._generalLogViewRequiredPermissionSetId, ex);
        }

        void WriteTrackingMessages(bool writeBusinessTracking, bool writeToParent, LogEntryType severity, string messageFormat, params object[] args)
        {
            if (writeToParent && !this.InstanceInfo.ParentProcessID.HasValue)
                throw new InvalidOperationException("Cannot write tracking to parent process while parent process id is null");

            BPTrackingMessage trackingMessage = new BPTrackingMessage
            {
                TrackingMessage = args != null && args.Length > 0 ? String.Format(messageFormat, args) : messageFormat,
                EventTime = DateTime.Now,
                ProcessInstanceId = writeToParent ? this.InstanceInfo.ParentProcessID.Value : this.InstanceInfo.ProcessInstanceID,
                ParentProcessId = writeToParent ? null : this.InstanceInfo.ParentProcessID,
                Severity = severity
            };
            BPTrackingChannel.Current.WriteTrackingMessage(trackingMessage);
            if (writeBusinessTracking)
                LoggerFactory.GetLogger().WriteEntry(_generalLogEventType, this._generalLogViewRequiredPermissionSetId, severity, messageFormat, args);
        }
    }
}
