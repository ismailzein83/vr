using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;

namespace TOne.WhS.DBSync.Entities
{
    public class MigrationContext
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public Dictionary<DBTableName, DBTable> DBTables { get; set; }
        public List<DBTableName> MigrationRequestedTables { get; set; }
        public MigrationCredentials MigrationCredentials { get; set; }
        public int DefaultSellingNumberPlanId { get; set; }

        public int SellingProductId { get; set; }

        public int OffPeakRateTypeId { get; set; }
        public int WeekendRateTypeId { get; set; }

        public bool MigratePriceListData { get; set; }
        public void WriteException(Exception ex)
        {
            LoggerFactory.GetExceptionLogger().WriteException(ex);
        }

        public void WriteInformation(string message)
        {
            LoggerFactory.GetLogger().WriteInformation(message);
        }

        public void WriteEntry(Vanrise.Entities.LogEntryType logEntryType, string message)
        {
            LoggerFactory.GetLogger().WriteEntry(logEntryType, message);
        }

        public void WriteVerbose(string message)
        {
            LoggerFactory.GetLogger().WriteVerbose(message);
        }

        public void WriteError(string message)
        {
            LoggerFactory.GetLogger().WriteError(message);
        }

        public void WriteWarning(string message)
        {
            LoggerFactory.GetLogger().WriteWarning(message);
        }
    }
}
