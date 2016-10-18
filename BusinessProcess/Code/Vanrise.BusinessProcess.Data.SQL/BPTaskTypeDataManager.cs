using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPTaskTypeDataManager : BaseSQLDataManager, IBPTaskTypeDataManager
    {
        public BPTaskTypeDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }
        #region public methods
        public IEnumerable<BPTaskType> GetBPTaskTypes()
        {
            return GetItemsSP("bp.sp_BPTaskType_GetAll", BPTaskTypeMapper);
        }

        public bool AreBPTaskTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPTaskType]", ref updateHandle);
        }
        #endregion

        #region Mappers

        BPTaskType BPTaskTypeMapper(IDataReader reader)
        {
            var bpTaskType = new BPTaskType
            {
                BPTaskTypeId =  GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string
            };
            string settings = reader["Settings"] as string;
            if (!String.IsNullOrWhiteSpace(settings))
                bpTaskType.Settings = Serializer.Deserialize<BPTaskTypeSettings>(settings);

            return bpTaskType;
        }
        #endregion
    }
}