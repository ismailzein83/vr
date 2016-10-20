﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Deal.Data.SQL
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

        public List<TOne.WhS.Deal.Entities.Deal> GetDeals()
        {
            return GetItemsSP("TOneWhS_Deal.sp_Deal_GetAll", DealMapper);
        }

        public bool AreDealsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Deal.Deal", ref updateHandle);
        }

        public bool Insert(TOne.WhS.Deal.Entities.Deal deal, out int insertedId)
        {
            object dealId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_Deal.sp_Deal_Insert", out dealId, deal.Name, Vanrise.Common.Serializer.Serialize(deal.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)dealId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }

        public bool Update(TOne.WhS.Deal.Entities.Deal deal)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_Deal.sp_Deal_Update", deal.DealId, deal.Name, Vanrise.Common.Serializer.Serialize(deal.Settings));
            return (recordsEffected > 0);
        }

        #endregion

        #region  Mappers

        private TOne.WhS.Deal.Entities.Deal DealMapper(IDataReader reader)
        {
            TOne.WhS.Deal.Entities.Deal deal = new TOne.WhS.Deal.Entities.Deal
            {
                DealId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<DealSettings>(reader["Settings"] as string)
            };
            return deal;
        }

        #endregion
    }
}
