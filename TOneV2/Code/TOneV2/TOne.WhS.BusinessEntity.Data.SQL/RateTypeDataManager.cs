using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using System.Data;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class RateTypeDataManager : BaseSQLDataManager, IRateTypeDataManager
    {
        public RateTypeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        { }

        public List<Entities.RateType> GetRateTypes()
        {
            return GetItemsSP("[TOneWhS_BE].[sp_RateType_GetAll]", RateTypeMapper);
        }

        public bool Update(Entities.RateType rateType)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_RateType_Update]", rateType.Name);
            return (recordsEffected > 0);
        }

        public bool Insert(Entities.RateType rateType, out int insertedId)
        {
            object rateTypeId;
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_RateType_Insert]", out rateTypeId, rateType.Name);
            insertedId = (int)rateTypeId;
            return (recordsEffected > 0);
        }

        public bool AreRateTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.RateType", ref updateHandle);
        }

        # region Mappers

        RateType RateTypeMapper(IDataReader reader)
        {
            RateType rateType = new RateType();
            rateType.Name = reader["Name"] as string;
            rateType.RateTypeId = (int)reader["ID"];
            return rateType;
        }

        #endregion



    }
}
