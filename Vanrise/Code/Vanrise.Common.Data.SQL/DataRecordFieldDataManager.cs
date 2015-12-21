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
    public class DataRecordFieldDataManager : BaseSQLDataManager, IDataRecordFieldDataManager
    {
      
        #region ctor/Local Variables
        public DataRecordFieldDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public List<Entities.GenericDataRecord.DataRecordField> GetALllDataRecordFields()
        {
            return GetItemsSP("common.sp_DataRecordField_GetAll", DataRecordTypeMapper);
        }
        public bool AreDataRecordFieldUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[DataRecordField]", ref updateHandle);
        }
        public bool Update(DataRecordField dataRecordField)
        {
            string serializedObject = Serializer.Serialize(dataRecordField);
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_DataRecordField_Update]", dataRecordField.ID, serializedObject);
            return (recordsEffected > 0);
        }
        public bool Insert(DataRecordField dataRecordField, out int insertedId)
        {
            object dataRecordFieldId;
            string serializedObject = Serializer.Serialize(dataRecordField);

            int recordsEffected = ExecuteNonQuerySP("[common].[sp_DataRecordField_Insert]", out dataRecordFieldId, dataRecordField.DataRecordTypeID, serializedObject);
            insertedId = (int)dataRecordFieldId;
            return (recordsEffected > 0);
        }
        public bool Delete(int dataRecordFieldId)
        {
            int recordesEffected = ExecuteNonQuerySP("[common].[sp_DataRecordField_Delete]", dataRecordFieldId);
            return (recordesEffected > 0);
        }
        #endregion

        #region Private Methods
        private DataRecordField DataRecordTypeMapper(IDataReader reader)
        {
            DataRecordField dataRecordField = reader["Details"] as string != null ? Serializer.Deserialize<DataRecordField>(reader["Details"] as string) : null;
            dataRecordField.ID = (int)reader["ID"];
            return dataRecordField;
        }

        #endregion
             
    }

}
