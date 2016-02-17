using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class OperatorDeclaredInformationDataManager : BaseSQLDataManager, IOperatorDeclaredInformationDataManager
    {
   
        #region ctor/Local Variables
        public OperatorDeclaredInformationDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(OperatorDeclaredInformation info, out int insertedId)
        {
            object infoId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorDeclaredInformation_Insert", out infoId, info.OperatorId, info.FromDate, info.ToDate, info.ZoneId, info.Volume, info.AmountType);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)infoId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(OperatorDeclaredInformation info)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorDeclaredInformation_Update", info.OperatorDeclaredInformationId, info.OperatorId, info.FromDate, info.ToDate, info.ZoneId, info.Volume, info.AmountType);
            return (recordsEffected > 0);
        }
        public bool AreOperatorDeclaredInformationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.OperatorDeclaredInformation", ref updateHandle);
        }
        public List<OperatorDeclaredInformation> GetOperatorDeclaredInformations()
        {
            return GetItemsSP("dbo.sp_OperatorDeclaredInformation_GetAll", OperatorDeclaredInformationMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private OperatorDeclaredInformation OperatorDeclaredInformationMapper(IDataReader reader)
        {
            OperatorDeclaredInformation info = new OperatorDeclaredInformation
            {
                OperatorDeclaredInformationId = (int)reader["ID"],
                OperatorId = (int)reader["OperatorID"],
                FromDate = GetReaderValue<DateTime?>(reader,"FromDate"),
                ToDate = GetReaderValue<DateTime?>(reader,"ToDate"),
                ZoneId = GetReaderValue<long?>(reader, "ZoneId"),
                Volume = GetReaderValue<int>(reader, "Volume"),
                AmountType = GetReaderValue<int>(reader, "AmountType"),
            };
            return info;
        }

        #endregion
      
    }
}
