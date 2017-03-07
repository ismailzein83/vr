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
        public Dictionary<string, ParameterValue> ParameterDefinitions { get; set; }
        public int SellingProductId { get; set; }
        public bool IsCustomerCommissionNegative { get; set; }
        public int OffPeakRateTypeId { get; set; }
        public int WeekendRateTypeId { get; set; }
        public int HolidayRateTypeId { get; set; }
        public bool MigratePriceListData { get; set; }
        public bool OnlyEffective { get; set; }


        public virtual void WriteException(Exception ex)
        {
            LoggerFactory.GetExceptionLogger().WriteException(ex);
        }

        public virtual void WriteInformation(string message)
        {
            LoggerFactory.GetLogger().WriteInformation(message);
        }

        public virtual void WriteEntry(Vanrise.Entities.LogEntryType logEntryType, string message)
        {
            LoggerFactory.GetLogger().WriteEntry(logEntryType, message);
        }

        public virtual void WriteVerbose(string message)
        {
            LoggerFactory.GetLogger().WriteVerbose(message);
        }

        public virtual void WriteError(string message)
        {
            LoggerFactory.GetLogger().WriteError(message);
        }

        public virtual void WriteWarning(string message)
        {
            LoggerFactory.GetLogger().WriteWarning(message);
        }
    }
}
