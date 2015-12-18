using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.Data.SQL
{
    public class DataRecordTypeDataManager : BaseSQLDataManager, IDataRecordTypeDataManager
    {
        public DataRecordTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
        }

        public List<Entities.GenericDataRecord.DataRecordType> GetALllDataRecordTypes()
        {
            return GetItemsSP("common.sp_DataRecordType_GetAll", DataRecordTypeMapper);
        }

        DataRecordType DataRecordTypeMapper(IDataReader reader)
        {
            DataRecordType dataRecordType = new DataRecordType
            {
                DataRecordTypeId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Fields =reader["ConfigDetail"] as string!=null? Serializer.Deserialize<List<DataRecordField>>(reader["ConfigDetail"] as string):null,
                ParentId = GetReaderValue<int?>(reader, "ParentID")
            };
            return dataRecordType;
        }

        public bool AreDataRecordTypeUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[DataRecordType]", ref updateHandle);
        }
    }
}
