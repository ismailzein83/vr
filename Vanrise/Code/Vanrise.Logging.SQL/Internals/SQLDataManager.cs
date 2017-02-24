using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Logging.SQL
{
    public class SQLDataManager : BaseSQLDataManager
    {
        public SQLDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {

        }
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        static SQLDataManager()
        {
            _columnMapper.Add("LogEntryId", "ID");
            _columnMapper.Add("MachineName", "MachineNameId");
            _columnMapper.Add("ApplicationName", "ApplicationNameId");
            _columnMapper.Add("AssemblyName", "AssemblyNameId");
            _columnMapper.Add("TypeName", "TypeNameId");
            _columnMapper.Add("EntryTypeName", "EntryType");
            _columnMapper.Add("MethodName", "MethodNameId");
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
            dt.Columns.Add("EventType", typeof(int));
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("ExceptionDetail", typeof(string));
            dt.Columns.Add("EventTime", typeof(DateTime));
            dt.BeginLoadData();
            foreach (var e in entries)
            {
                DataRow dr = dt.NewRow();
                dr["MachineNameId"] = machineNameId;
                dr["ApplicationNameId"] = applicationNameId;
                dr["AssemblyNameId"] = GetAttributeId(LogAttributeType.AssemblyName, e.AssemblyName);
                dr["TypeNameId"] = GetAttributeId(LogAttributeType.TypeName, e.TypeName);
                dr["MethodNameId"] = GetAttributeId(LogAttributeType.MethodName, e.MethodName);
                dr["EntryType"] = (int)e.EntryType;
                dr["EventType"] = GetAttributeId(LogAttributeType.EventType, e.EventType);
                dr["Message"] = e.Message;
                dr["ExceptionDetail"] = e.ExceptionDetail;
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


        public Vanrise.Entities.BigResult<Vanrise.Entities.LogEntry> GetFilteredLogs(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.LogEntryQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                //string zoneIds = null;
                //if (input.Query.ZoneIds != null && input.Query.ZoneIds.Count() > 0)
                //    zoneIds = string.Join<int>(",", input.Query.ZoneIds);

                string machineIds = null;
                if (input.Query.MachineIds != null && input.Query.MachineIds.Count() > 0)
                    machineIds = string.Join<int>(",", input.Query.MachineIds);

                string applicationIds = null;
                if (input.Query.ApplicationIds != null && input.Query.ApplicationIds.Count() > 0)
                    applicationIds = string.Join<int>(",", input.Query.ApplicationIds);

                string assemblyIds = null;
                if (input.Query.AssemblyIds != null && input.Query.AssemblyIds.Count() > 0)
                    assemblyIds = string.Join<int>(",", input.Query.AssemblyIds);

                string typeIds = null;
                if (input.Query.TypeIds != null && input.Query.TypeIds.Count() > 0)
                    typeIds = string.Join<int>(",", input.Query.TypeIds);

                string methodIds = null;
                if (input.Query.MethodIds != null && input.Query.MethodIds.Count() > 0)
                    methodIds = string.Join<int>(",", input.Query.MethodIds);

                string entryTypeIds = null;
                if (input.Query.EntryType != null && input.Query.EntryType.Count() > 0)
                    entryTypeIds = string.Join<int>(",", input.Query.EntryType);

                ExecuteNonQuerySP("[logging].[sp_LogEntry_CreateTempByFiltered]", tempTableName, entryTypeIds, input.Query.Message, input.Query.FromDate, input.Query.ToDate, machineIds, applicationIds, assemblyIds, typeIds, methodIds);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, LogEntryMapper, _columnMapper);
        }

        Vanrise.Entities.LogEntry LogEntryMapper(IDataReader reader)
        {
            Vanrise.Entities.LogEntry logEntry = new Entities.LogEntry()
            {
                LogEntryId = (long)reader["ID"],
                MachineId = (int)reader["MachineNameId"],
                ApplicationId = (int)reader["ApplicationNameId"],
                AssemblyId = (int)reader["AssemblyNameId"],
                TypeId = (int)reader["TypeNameId"],
                MethodId = (int)reader["MethodNameId"],
                EntryType = (LogEntryType)reader["EntryType"],
                Message = reader["Message"] as string,
                EventTime = GetReaderValue<DateTime>(reader, "EventTime")

            };
            return logEntry;
        }

        public List<LogAttribute> GetLogAttributes()
        {
            return GetItemsSP("[logging].[sp_LogAttribute_GetAll]", LogAttributeMapper);
        }

        LogAttribute LogAttributeMapper(IDataReader reader)
        {
            LogAttribute logAttribute = new LogAttribute
            {
                LogAttributeID = (int)reader["ID"],
                AttributeType = (int)reader["AttributeType"],
                Description = reader["Description"] as string
            };
            return logAttribute;
        }

        public bool AreLogAttributesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[logging].[LogAttribute]", ref updateHandle);
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
            if (!logAttributesByDescription.TryGetValue(attributeDescription, out attributeId))
            {
                attributeId = (int)ExecuteScalarSP("[logging].[sp_LogAttribute_InsertIfNeededAndGetID]", (int)attributeType, attributeDescription);
                logAttributesByDescription.TryAdd(attributeDescription, attributeId);
            }
            return attributeId;
        }
    }
}
