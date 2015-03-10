using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Logging.SQL
{
    internal class SQLDataManager : BaseSQLDataManager
    {
        public SQLDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {
            
        }

        LogAttributesByType _logAttributes;

        public void WriteEntries(string machineName, string applicationName, List<LogEntry> entries)
        {
            LoadLogAttributesIfNotLoaded();
            DataTable dtEntries = ConvertEntriesToTable(machineName, applicationName, entries);
            WriteDataTableToDB(dtEntries, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        private DataTable ConvertEntriesToTable(string machineName, string applicationName, List<LogEntry> entries)
        {
            int machineNameId = GetAttributeId(LogAttributeType.MachineName, machineName);
            int applicationNameId = GetAttributeId(LogAttributeType.ApplicationName, applicationName);
            DataTable dt = new DataTable("logging.LogEntry");
            dt.Columns.Add("MachineNameId", typeof(int));
            dt.Columns.Add("ApplicationNameId", typeof(int));
            dt.Columns.Add("AssemblyNameId", typeof(int));
            dt.Columns.Add("TypeNameId", typeof(int));
            dt.Columns.Add("MethodNameId", typeof(int));
            dt.Columns.Add("EntryType", typeof(int));
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("EventTime", typeof(DateTime));
            dt.BeginLoadData();
            foreach(var e in entries)
            {
                DataRow dr = dt.NewRow();
                dr["MachineNameId"] = machineNameId;
                dr["ApplicationNameId"] = applicationNameId;
                dr["AssemblyNameId"] = GetAttributeId(LogAttributeType.AssemblyName, e.AssemblyName);
                dr["TypeNameId"] = GetAttributeId(LogAttributeType.TypeName, e.TypeName);
                dr["MethodNameId"] = GetAttributeId(LogAttributeType.MethodName, e.MethodName);
                dr["EntryType"] = (int)e.EntryType;
                dr["Message"] = e.Message;
                dr["EventTime"] = e.EventTime;
                dt.Rows.Add(dr);
            }
            dt.EndLoadData();

            return dt;
        }
        
        void LoadLogAttributesIfNotLoaded()
        {
            if (_logAttributes == null)
            {
                lock (this)
                {
                    if (_logAttributes == null)
                    {
                        var logAttributesByType = new LogAttributesByType();
                        ExecuteReaderSP("[logging].[sp_LogAttribute_GetAll]",
                            (reader) =>
                            {
                                while (reader.Read())
                                {
                                    LogAttributeType attributeType = (LogAttributeType)reader["AttributeType"];
                                    LogAttributesByDescription logAttributesByDescription;
                                    if (!logAttributesByType.TryGetValue(attributeType, out logAttributesByDescription))
                                    {
                                        logAttributesByDescription = new LogAttributesByDescription();
                                        logAttributesByType.TryAdd(attributeType, logAttributesByDescription);
                                    }
                                    string attributeDescription = reader["Description"] as string;
                                    if (!logAttributesByDescription.ContainsKey(attributeDescription))
                                        logAttributesByDescription.TryAdd(attributeDescription, (int)reader["ID"]);
                                }
                            });
                        _logAttributes = logAttributesByType;
                    }
                }
            }
        }

        int GetAttributeId(LogAttributeType attributeType, string attributeDescription)
        {
            LogAttributesByDescription logAttributesByDescription;
            if (!_logAttributes.TryGetValue(attributeType, out logAttributesByDescription))
            {
                logAttributesByDescription = new LogAttributesByDescription();
                _logAttributes.TryAdd(attributeType, logAttributesByDescription);
            }
            int attributeId;
            if(!logAttributesByDescription.TryGetValue(attributeDescription, out attributeId))
            {
                attributeId = (int)ExecuteScalarSP("[logging].[sp_LogAttribute_InsertIfNeededAndGetID]", (int)attributeType, attributeDescription);
                logAttributesByDescription.TryAdd(attributeDescription, attributeId);
            }
            return attributeId;
        }
    }
}
