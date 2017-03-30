﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Logging.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class LogManager
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

        public Vanrise.Entities.IDataRetrievalResult<Vanrise.Entities.LogEntryDetail> GetFilteredLogs(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.LogEntryQuery> input)
        {
            List<int> grantedPermissionSetIds;
            var requiredPermissionSetManager = Vanrise.Security.Entities.BEManagerFactory.GetManager<IRequiredPermissionSetManager>();
            bool isUserGrantedAllModulePermissionSets = requiredPermissionSetManager.IsCurrentUserGrantedAllModulePermissionSets(LoggerFactory.LOGGING_REQUIREDPERMISSIONSET_MODULENAME, out grantedPermissionSetIds);
                    
            SQLDataManager manager = GetDataManager();

            BigResult<Vanrise.Entities.LogEntry> loggerResult = manager.GetFilteredLogs(input, grantedPermissionSetIds);

            BigResult<Vanrise.Entities.LogEntryDetail> loggerDetailResult = new BigResult<Vanrise.Entities.LogEntryDetail>()
            {
                ResultKey = loggerResult.ResultKey,
                TotalCount = loggerResult.TotalCount,
                Data = loggerResult.Data.MapRecords(LoggerDetailMapper)
            };

            ResultProcessingHandler<LogEntryDetail> handler = new ResultProcessingHandler<LogEntryDetail>()
            {
                ExportExcelHandler = new LogEntryExcelExportHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, loggerDetailResult, handler);
        }

        public IEnumerable<LogAttribute> GeLogAttributesById(int attribute)
        {
            return GetCachedLogAttributes().FindAllRecords(itm => itm.AttributeType == attribute).OrderBy(x => x.Description);
        }
        private Dictionary<int, LogAttribute> GetCachedLogAttributes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetLogAttributes"),
               () =>
               {
                   SQLDataManager dataManager = GetDataManager();
                   List<LogAttribute> allLogAttributes = dataManager.GetLogAttributes();
                   return allLogAttributes.ToDictionary(l => l.LogAttributeID, l => l);
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
                EventTypeName = (logEntry.EventType.HasValue)?this.GetAttributeName(logEntry.EventType.Value):null,
                EntryTypeName = Vanrise.Common.Utilities.GetEnumDescription(logEntry.EntryType),
            };
        }

        private string GetAttributeName(int attributeId)
        {
            LogAttribute logAttribute = GetCachedLogAttributes().GetRecord(attributeId);

            if (logAttribute != null)
                return logAttribute.Description;

            return null;
        }

        private class LogEntryExcelExportHandler : ExcelExportHandler<LogEntryDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<LogEntryDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Log Entry",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Event Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Level" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Event Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Message", Width = 120});

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.LogEntryId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EventTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.EntryTypeName });
                            row.Cells.Add(new ExportExcelCell { Value = record.EventTypeName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Message });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }
    }
}
