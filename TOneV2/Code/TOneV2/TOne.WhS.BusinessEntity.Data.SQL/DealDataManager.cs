using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class DealDataManager : BaseSQLDataManager, IDealDataManager
    {
        #region Constructors

        public DealDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<Deal> GetDeals()
        {
            return GetItemsSP("TOneWhS_BE.sp_Deal_GetAll", DealMapper);
        }

        public bool AreDealsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.Deal", ref updateHandle);
        }

        public bool Insert(Deal deal, out int insertedId)
        {
            object dealId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_Deal_Insert", out dealId, Vanrise.Common.Serializer.Serialize(deal.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)dealId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public bool Update(Deal deal)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_Deal_Update", deal.DealId, Vanrise.Common.Serializer.Serialize(deal.Settings));
            return (recordsEffected > 0);
        }

        #endregion

        #region  Mappers

        private Deal DealMapper(IDataReader reader)
        {
            Deal deal = new Deal
            {
                DealId = (int)reader["ID"],
                Settings = Vanrise.Common.Serializer.Deserialize<DealSettings>(reader["Settings"] as string)
            };
            return deal;
        }

        #endregion
    }
}
