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
            _columnMapper.Add("EventTypeName", "EventType");
        }

        public void AddEntry(Func<LogAttributeType, string, int> getAttributeId, string machineName, string applicationName, string assemblyName, string typeName,
            string methodName, LogEntryType entryType, string eventType, int? viewRequiredPermissionSetId, string message, string exceptionDetail, DateTime eventTime)
        {
            int machineNameId = getAttributeId(LogAttributeType.MachineName, machineName);
            int applicationNameId = getAttributeId(LogAttributeType.ApplicationName, applicationName);
            ExecuteNonQuerySP("logging.sp_LogEntry_Insert", machineNameId, applicationNameId, getAttributeId(LogAttributeType.AssemblyName, assemblyName), getAttributeId(LogAttributeType.TypeName, typeName),
                getAttributeId(LogAttributeType.MethodName, methodName), (int)entryType, getAttributeId(LogAttributeType.EventType, eventType),
                viewRequiredPermissionSetId.HasValue ? (Object)viewRequiredPermissionSetId.Value : DBNull.Value,
                message, exceptionDetail, eventTime);
        }

        public Vanrise.Entities.BigResult<Vanrise.Entities.LogEntry> GetFilteredLogs(Vanrise.Entities.DataRetrievalInput<Vanrise.Entities.LogEntryQuery> input, List<int> grantedPermissionSetIds)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
              
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

                string eventTypeIds = null;
                if (input.Query.EventType != null && input.Query.EventType.Count() > 0)
                    eventTypeIds = string.Join<int>(",", input.Query.EventType);

                string grantedPermissionSetIdsAsString = null;
                if (grantedPermissionSetIds != null )
                    grantedPermissionSetIdsAsString = string.Join<int>(",", grantedPermissionSetIds);

                ExecuteNonQuerySP("[logging].[sp_LogEntry_CreateTempByFiltered]", tempTableName, entryTypeIds, input.Query.Message, input.Query.FromDate, input.Query.ToDate, machineIds, applicationIds, assemblyIds, typeIds, methodIds, eventTypeIds, grantedPermissionSetIdsAsString);
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
                ExceptionDetail = reader["ExceptionDetail"] as string,
                EventTime = GetReaderValue<DateTime>(reader, "EventTime"),
                EventType = GetReaderValue<int?>(reader,"EventType")

            };
            return logEntry;
        }

 
    }
}
