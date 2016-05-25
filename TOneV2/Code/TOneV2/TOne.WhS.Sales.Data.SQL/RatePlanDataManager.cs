﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RatePlanDataManager : BaseTOneDataManager, IRatePlanDataManager
    {
        public RatePlanDataManager() : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString")) { }

        public bool InsertPriceList(SalePriceList priceList, out int priceListId)
        {
            object insertedId;

            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_SalePriceList_Insert", out insertedId, priceList.OwnerType, priceList.OwnerId, priceList.CurrencyId);

            priceListId = (int)insertedId;

            return affectedRows > 0;
        }

        public bool UpdateRatePlanStatus(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus existingStatus, RatePlanStatus newStatus)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_UpdateStatus", ownerType, ownerId, existingStatus, newStatus);
            return affectedRows == 1;
        }

        #region Get Changes

        public Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            return GetItemSP("TOneWhS_Sales.sp_RatePlan_GetChanges", ChangesMapper, ownerType, ownerId, status);
        }

        private Changes ChangesMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<Changes>(reader["Changes"] as string);
        }
        
        #endregion

        public bool InsertOrUpdateChanges(SalePriceListOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status)
        {
            string serializedChanges = Vanrise.Common.Serializer.Serialize(changes);

            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_InsertOrUpdateChanges", ownerType, ownerId, serializedChanges, status);
            return affectedRows > 0;
        }

        public bool CancelRatePlanChanges(SalePriceListOwnerType ownerType, int ownerId)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_CancelChanges", ownerType, ownerId);
            return affectedRows > 0;
        }
    }
}
