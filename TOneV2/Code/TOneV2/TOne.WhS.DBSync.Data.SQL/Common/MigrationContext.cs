﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class MigrationContext
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public Dictionary<DBTableName, DBTable> DBTables { get; set; }
        public MigrationCredentials MigrationCredentials { get; set; }
        public int DefaultSellingNumberPlanId { get; set; }


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
