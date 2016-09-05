using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    public class OperatorDeclaredInfoDataManager :BaseSQLDataManager, IOperatorDeclaredInfoDataManager
    {
           
        #region ctor/Local Variables
        public OperatorDeclaredInfoDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "Retail_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public bool AreOperatorDeclaredInfosUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.OperatorDeclaredInfo", ref updateHandle);
        }
        public List<OperatorDeclaredInfo> GetOperatorDeclaredInfos()
        {
            return GetItemsSP("Retail.sp_OperatorDeclaredInfo_GetAll", OperatorDeclaredInfoMapper);
        }
         public bool Insert(OperatorDeclaredInfo OperatorDeclaredInfo, out int insertedId)
        {
            object OperatorDeclaredInfoId;
            string serializedSettings = OperatorDeclaredInfo.Settings != null ? Vanrise.Common.Serializer.Serialize(OperatorDeclaredInfo.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_OperatorDeclaredInfo_Insert", out OperatorDeclaredInfoId,  serializedSettings);
            insertedId = (affectedRecords > 0) ? (int)OperatorDeclaredInfoId : -1;
            return (affectedRecords > 0);
        }

        public bool Update(OperatorDeclaredInfo OperatorDeclaredInfo)
        {
            string serializedSettings = OperatorDeclaredInfo.Settings != null ? Vanrise.Common.Serializer.Serialize(OperatorDeclaredInfo.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail.sp_OperatorDeclaredInfo_Update", OperatorDeclaredInfo.OperatorDeclaredInfoId, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private OperatorDeclaredInfo OperatorDeclaredInfoMapper(IDataReader reader)
        {
           return new OperatorDeclaredInfo
            {
                OperatorDeclaredInfoId = (int)reader["ID"],
                Settings = Vanrise.Common.Serializer.Deserialize<OperatorDeclaredInfoSettings>(reader["Settings"] as string),
            };
        }

        #endregion
    }
}
