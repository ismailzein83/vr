using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Data.SQL
{
    public class RateTypeDataManager : BaseSQLDataManager, IRateTypeDataManager
    {
        #region ctor/Local Variables
        public RateTypeDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        { }

        #endregion

        #region Public Methods
        public List<Vanrise.Entities.RateType> GetRateTypes()
        {
            return GetItemsSP("[common].[sp_RateType_GetAll]", RateTypeMapper);
        }

        public bool Update(Vanrise.Entities.RateType rateType)
        {
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_RateType_Update]", rateType.RateTypeId, rateType.Name);
            return (recordsEffected > 0);
        }

        public bool Insert(Vanrise.Entities.RateType rateType, out int insertedId)
        {
            object rateTypeId;
            int recordsEffected = ExecuteNonQuerySP("[common].[sp_RateType_Insert]", out rateTypeId, rateType.Name);
            insertedId = (int)rateTypeId;
            return (recordsEffected > 0);
        }

        public bool AreRateTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.RateType", ref updateHandle);
        }
        #endregion

        #region Private Methods
        #endregion

        # region Mappers
        Vanrise.Entities.RateType RateTypeMapper(IDataReader reader)
        {
            Vanrise.Entities.RateType rateType = new Vanrise.Entities.RateType();
            rateType.Name = reader["Name"] as string;
            rateType.RateTypeId = (int)reader["ID"];
            return rateType;
        }
        #endregion
    }
}
