﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Logging.SQL;

namespace Vanrise.Common.Business
{
    public class LoggingManager
    {
        internal static SQLDataManager GetDataManager()
        {
            var logHandlers = LoggerFactory.GetLogger().LogHandlers;
            if (logHandlers != null)
            {
                foreach (var handler in logHandlers)
                {
                    SQLLogger sqlLogger = handler as SQLLogger;
                    if (sqlLogger != null)
                        return sqlLogger.DataManager;
                }
            }
            return null;
        }

        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Entities.LogEntryDetail> GetFilteredLoggers(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.LogEntryQuery> input)
        {
            SQLDataManager manager = GetDataManager();

            BigResult<Vanrise.Entities.LogEntry> loggerResult = manager.GetFilteredSupplierCodes(input);

            BigResult<Vanrise.Entities.LogEntryDetail> loggerDetailResult = new BigResult<Vanrise.Entities.LogEntryDetail>()
            {
                ResultKey = loggerResult.ResultKey,
                TotalCount = loggerResult.TotalCount,
                Data = loggerResult.Data.MapRecords(LoggerDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, loggerDetailResult);
        }

        public List<LogAttribute> GetSpecificLogAttribute(int attribute)
        {
            List<LogAttribute> specificLogAttribute = new List<LogAttribute>();
            specificLogAttribute = GetCachedLogAttributes().Where(l => l.AttributeType == attribute).ToList();
            return specificLogAttribute;
        }
        private List<LogAttribute> GetCachedLogAttributes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetLogAttributes"),
               () =>
               {
                   SQLDataManager dataManager = GetDataManager();
                   List<LogAttribute> allLogAttributes = dataManager.GetLogAttributes();
                   return allLogAttributes;
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            SQLDataManager _dataManager = GetDataManager();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreLogAttributesUpdated(ref _updateHandle);
            }
        }

        private Vanrise.Entities.LogEntryDetail LoggerDetailMapper(Vanrise.Entities.LogEntry logEntry)
        {
            return new LogEntryDetail()
            {
                Entity = logEntry,
                MachineName = this.GetAttributeName(logEntry.MachineId),
                ApplicationName = this.GetAttributeName(logEntry.ApplicationId),
                AssemblyName = this.GetAttributeName(logEntry.AssemblyId),
                TypeName = this.GetAttributeName(logEntry.TypeId),
                MethodName = this.GetAttributeName(logEntry.MethodId),
                EntryTypeName = Vanrise.Common.Utilities.GetEnumDescription(logEntry.EntryType)
            };
        }

        private string GetAttributeName(int attributeId)
        {
            SQLDataManager manager = GetDataManager();
            LogAttribute logAttribute = GetCachedLogAttributes().Where(l => l.LogAttributeID == attributeId).FirstOrDefault();

            if (logAttribute != null)
                return logAttribute.Description;

            return "Attribute Not Found";
        }
    }
}
