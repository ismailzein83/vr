using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanDraftManager
    {
        public Changes GetDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            IRatePlanDataManager ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
        }

        public bool DoesDraftExist(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            Changes changes = ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes == null)
                return false;

            if (changes.ZoneChanges != null)
                return true;

            if (changes.DefaultChanges != null)
                if (changes.DefaultChanges.NewDefaultRoutingProduct != null || changes.DefaultChanges.DefaultRoutingProductChange != null)
                    return true;

            return false;
        }

        #region Save Draft

        public void SaveDraft(SalePriceListOwnerType ownerType, int ownerId, Changes newChanges)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();

            Changes existingChanges = ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
            Changes allChanges = MergeChanges(existingChanges, newChanges);

            if (allChanges != null)
                ratePlanDataManager.InsertOrUpdateChanges(ownerType, ownerId, allChanges, RatePlanStatus.Draft);
        }

        Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges, () =>
            {
                Changes allChanges = new Changes();

                allChanges.DefaultChanges = newChanges.DefaultChanges == null ? existingChanges.DefaultChanges : newChanges.DefaultChanges;
                allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges);

                return allChanges;
            });
        }

        List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges)
        {
            return Merge(existingZoneChanges, newZoneChanges, () =>
            {
                foreach (ZoneChanges zoneItemChanges in existingZoneChanges)
                {
                    if (!newZoneChanges.Any(item => item.ZoneId == zoneItemChanges.ZoneId))
                        newZoneChanges.Add(zoneItemChanges);
                }
                return newZoneChanges;
            });
        }

        T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeLogic();
            return existingChanges != null ? existingChanges : newChanges;
        }

        #endregion

        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.CancelRatePlanChanges(ownerType, ownerId);
        }
    }
}
