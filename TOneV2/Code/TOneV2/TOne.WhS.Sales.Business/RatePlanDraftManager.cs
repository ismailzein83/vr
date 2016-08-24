using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

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

        private Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges, () =>
            {
                Changes allChanges = new Changes();

                allChanges.CurrencyId = newChanges.CurrencyId;
                allChanges.DefaultChanges = newChanges.DefaultChanges == null ? existingChanges.DefaultChanges : newChanges.DefaultChanges;
                allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges);

                return allChanges;
            });
        }

        private List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges)
        {
            return Merge(existingZoneChanges, newZoneChanges, () =>
            {
                foreach (ZoneChanges existingZoneDraft in existingZoneChanges)
                {
                    if (!newZoneChanges.Any(x => x.ZoneId == existingZoneDraft.ZoneId))
                        newZoneChanges.Add(existingZoneDraft);
                }
                return newZoneChanges;
            });
        }

        private IEnumerable<DraftRateToChange> MergeNewRates(IEnumerable<DraftRateToChange> existingNewRates, IEnumerable<DraftRateToChange> importedNewRates)
        {
            return Merge<IEnumerable<DraftRateToChange>>(existingNewRates, importedNewRates, () =>
            {
                var mergedNewRates = new List<DraftRateToChange>();

                DraftRateToChange existingNewNormalRate;
                List<DraftRateToChange> existingNewOtherRates;
                SetNormalAndOtherNewRates(existingNewRates, out existingNewNormalRate, out existingNewOtherRates);

                DraftRateToChange importedNewNormalRate;
                List<DraftRateToChange> importedNewOtherRates;
                SetNormalAndOtherNewRates(importedNewRates, out importedNewNormalRate, out importedNewOtherRates);

                if (importedNewNormalRate != null)
                    mergedNewRates.Add(importedNewNormalRate);
                else if (existingNewNormalRate != null)
                    mergedNewRates.Add(existingNewNormalRate);

                if (importedNewOtherRates.Count > 0)
                    mergedNewRates.AddRange(importedNewOtherRates);
                else if (existingNewOtherRates.Count > 0)
                    mergedNewRates.AddRange(existingNewOtherRates);

                return mergedNewRates;
            });
        }

        private IEnumerable<DraftRateToClose> MergeClosedRates(IEnumerable<DraftRateToClose> existingClosedRates, IEnumerable<DraftRateToClose> newClosedRates)
        {
            return Merge<IEnumerable<DraftRateToClose>>(existingClosedRates, newClosedRates, () =>
            {
                var mergedClosedRates = new List<DraftRateToClose>();

                DraftRateToClose existingClosedNormalRate;
                List<DraftRateToClose> existingClosedOtherRates;
                SetNormalAndOtherClosedRates(existingClosedRates, out existingClosedNormalRate, out existingClosedOtherRates);

                DraftRateToClose newClosedNormalRate;
                List<DraftRateToClose> newClosedOtherRates;
                SetNormalAndOtherClosedRates(newClosedRates, out newClosedNormalRate, out newClosedOtherRates);

                if (newClosedNormalRate != null)
                    mergedClosedRates.Add(newClosedNormalRate);
                else if (existingClosedNormalRate != null)
                    mergedClosedRates.Add(existingClosedNormalRate);

                if (newClosedOtherRates.Count > 0)
                    mergedClosedRates.AddRange(newClosedOtherRates);
                else if (existingClosedOtherRates.Count > 0)
                    mergedClosedRates.AddRange(existingClosedOtherRates);

                return mergedClosedRates;
            });
        }

        private T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeLogic();
            return existingChanges != null ? existingChanges : newChanges;
        }

        private void SetNormalAndOtherNewRates(IEnumerable<DraftRateToChange> newRates, out DraftRateToChange newNormalRate, out List<DraftRateToChange> newOtherRates)
        {
            newNormalRate = null;
            newOtherRates = new List<DraftRateToChange>();

            foreach (DraftRateToChange newRate in newRates)
            {
                if (newRate.RateTypeId.HasValue)
                    newOtherRates.Add(newRate);
                else
                    newNormalRate = newRate;
            }
        }

        private void SetNormalAndOtherClosedRates(IEnumerable<DraftRateToClose> closedRates, out DraftRateToClose closedNormalRate, out List<DraftRateToClose> closedOtherRates)
        {
            closedNormalRate = new DraftRateToClose();
            closedOtherRates = new List<DraftRateToClose>();

            foreach (DraftRateToClose closedRate in closedRates)
            {
                if (closedRate.RateTypeId.HasValue)
                    closedOtherRates.Add(closedRate);
                else
                    closedNormalRate = closedRate;
            }
        }

        #endregion

        public bool DeleteDraft(SalePriceListOwnerType ownerType, int ownerId)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.CancelRatePlanChanges(ownerType, ownerId);
        }

        public int? GetDraftCurrencyId(SalePriceListOwnerType ownerType, int ownerId)
        {
            Changes draft = GetDraft(ownerType, ownerId);
            if (draft != null)
                return draft.CurrencyId;
            return null;
        }

        public void DeleteChangedRates(SalePriceListOwnerType ownerType, int ownerId, int newCurrencyId)
        {
            Changes draft = GetDraft(ownerType, ownerId);
            
            if (draft != null)
            {
                draft.CurrencyId = newCurrencyId;
                
                if (draft.ZoneChanges != null)
                {
                    foreach (ZoneChanges zoneDraft in draft.ZoneChanges)
                        zoneDraft.NewRates = null;
                }

                SaveDraft(ownerType, ownerId, draft);
            }
        }
    }
}
