﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class DataRecordFieldChoiceDataManager : BaseSQLDataManager, IDataRecordFieldChoiceDataManager
    {
        public DataRecordFieldChoiceDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
        #region Public Methods

        public IEnumerable<DataRecordFieldChoice> GetDataRecordFieldChoices()
        {
            return GetItemsSP("genericdata.sp_DataRecordFieldChoice_GetAll", DataRecordFieldChoiceMapper);
        }

        public bool AddDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice, out int insertedId)
        {
            object dataRecordFieldChoiceId;

            int affectedRows = ExecuteNonQuerySP("genericdata.sp_DataRecordFieldChoice_Insert", out dataRecordFieldChoiceId, dataRecordFieldChoice.Name, Vanrise.Common.Serializer.Serialize(dataRecordFieldChoice.Settings));
            insertedId = (affectedRows == 1) ? (int)dataRecordFieldChoiceId : -1;

            return (affectedRows == 1);
        }
        public bool UpdateDataRecordFieldChoice(DataRecordFieldChoice dataRecordFieldChoice)
        {
            int affectedRows = ExecuteNonQuerySP("genericdata.sp_DataRecordFieldChoice_Update", dataRecordFieldChoice.DataRecordFieldChoiceId, dataRecordFieldChoice.Name, Vanrise.Common.Serializer.Serialize(dataRecordFieldChoice.Settings));
            return (affectedRows == 1);
        }
        public bool AreDataRecordFieldChoicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.DataRecordFieldChoice", ref updateHandle);
        }

        #endregion
          
        #region Mappers

        DataRecordFieldChoice DataRecordFieldChoiceMapper(IDataReader reader)
        {
            return new DataRecordFieldChoice()
            {
                DataRecordFieldChoiceId = (int)reader["ID"],
                Name = (string)reader["Name"],
                Settings = Vanrise.Common.Serializer.Deserialize<DataRecordFieldChoiceSettings>((string)reader["Settings"])
            };
        }
        
        #endregion
    }

}
