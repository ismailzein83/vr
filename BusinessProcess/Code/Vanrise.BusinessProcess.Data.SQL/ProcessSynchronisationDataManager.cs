using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class ProcessSynchronisationDataManager : BaseSQLDataManager, IProcessSynchronisationDataManager
    {
        public ProcessSynchronisationDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #region Public Methods

        public List<ProcessSynchronisation> GetProcessSynchronisations()
        {
            return GetItemsSP("bp.sp_ProcessSynchronisation_GetAll", ProcessSynchronisationMapper);
        }

        public bool InsertProcessSynchronisation(Guid processSynchronisationId, ProcessSynchronisationToAdd processSynchronisationToAdd, int createdBy)
        {
            string serializedSettings = processSynchronisationToAdd.Settings != null ? Vanrise.Common.Serializer.Serialize(processSynchronisationToAdd.Settings) : null;
            return ExecuteNonQuerySP("[bp].[sp_ProcessSynchronisation_Insert]", processSynchronisationId, processSynchronisationToAdd.Name, serializedSettings, createdBy) > 0;
        }

        public bool UpdateProcessSynchronisation(ProcessSynchronisationToUpdate processSynchronisationToUpdate, int lastModifiedBy)
        {
            string serializedSettings = processSynchronisationToUpdate.Settings != null ? Vanrise.Common.Serializer.Serialize(processSynchronisationToUpdate.Settings) : null;
            return ExecuteNonQuerySP("[bp].[sp_ProcessSynchronisation_Update]", processSynchronisationToUpdate.ProcessSynchronisationId, processSynchronisationToUpdate.Name, serializedSettings, lastModifiedBy) > 0;
        }

        public bool AreProcessSynchronisationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[ProcessSynchronisation]", ref updateHandle);
        }

        #endregion

        #region Mappers

        ProcessSynchronisation ProcessSynchronisationMapper(IDataReader reader)
        {
            string settings = reader["Settings"] as string;
            return new ProcessSynchronisation
            {
                ProcessSynchronisationId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<ProcessSynchronisationSettings>(settings) : null,
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime")
            };
        }
        #endregion
    }
}