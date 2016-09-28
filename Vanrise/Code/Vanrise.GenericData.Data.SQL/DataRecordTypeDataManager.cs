using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class DataRecordTypeDataManager : BaseSQLDataManager,IDataRecordTypeDataManager
    {
        public DataRecordTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
        #region Public Methods
        public List<DataRecordType> GetDataRecordTypes()
        {
            return GetItemsSP("genericdata.sp_DataRecordType_GetAll", DataRecordTypeMapper);
        }

        public bool AreDataRecordTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataRecordType", ref updateHandle);
        }
        public bool UpdateDataRecordType(DataRecordType dataRecordType)
        {
            string serializedObj = null;
            if (dataRecordType.Fields != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(dataRecordType.Fields);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataRecordType_Update", dataRecordType.DataRecordTypeId, dataRecordType.Name, dataRecordType.ParentId, serializedObj);
            return (recordesEffected > 0);
        }

        public bool AddDataRecordType(DataRecordType dataRecordType)
        {

            string serializedObj = null;
            if (dataRecordType.Fields != null)
            {
                serializedObj = Vanrise.Common.Serializer.Serialize(dataRecordType.Fields);
            }
            int recordesEffected = ExecuteNonQuerySP("genericdata.sp_DataRecordType_Insert", dataRecordType.DataRecordTypeId, dataRecordType.Name, dataRecordType.ParentId, serializedObj);

            return (recordesEffected > 0);
        }
        #endregion

        #region Mappers

        DataRecordType DataRecordTypeMapper(IDataReader reader)
        {
            return new DataRecordType
            {
                DataRecordTypeId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                ParentId = GetReaderValue<Guid?>(reader, "ParentID"),
                Fields = Vanrise.Common.Serializer.Deserialize<List<DataRecordField>>(reader["Fields"] as string),
            };
        }

        #endregion


    
    }
}
