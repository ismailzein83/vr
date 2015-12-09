using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class RatePlanDataManager : BaseSQLDataManager, IRatePlanDataManager
    {
        public RatePlanDataManager() : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString")) { }

        #region Save Price List

        public bool InsertPriceList(SalePriceList priceList, out int priceListId)
        {
            object insertedId;

            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_SalePriceList_Insert", out insertedId, priceList.OwnerType, priceList.OwnerId, priceList.CurrencyId);

            priceListId = (int)insertedId;

            return affectedRows > 0;
        }

        public bool SetRatePlanStatusIfExists(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_SetStatusIfExists", ownerType, ownerId, status);
            return affectedRows > 0;
        }

        public IEnumerable<Changes> GetRecentChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            return GetItemsSP("TOneWhS_Sales.sp_RatePlan_GetRecent", ChangesMapper, ownerType, ownerId, status);
        }

        #endregion

        #region Changes

        public Changes GetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status)
        {
            return GetItemSP("TOneWhS_Sales.sp_RatePlan_GetChanges", ChangesMapper, ownerType, ownerId, status);
        }

        public bool InsertOrUpdateChanges(SalePriceListOwnerType ownerType, int ownerId, Changes changes, RatePlanStatus status)
        {
            string serializedChanges = Vanrise.Common.Serializer.Serialize(changes);

            int affectedRows = ExecuteNonQuerySP("TOneWhS_Sales.sp_RatePlan_InsertOrUpdateChanges", ownerType, ownerId, serializedChanges, status);
            return affectedRows > 0;
        }

        private Changes ChangesMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<Changes>(reader["Changes"] as string);
        }

        #endregion
    }
}
