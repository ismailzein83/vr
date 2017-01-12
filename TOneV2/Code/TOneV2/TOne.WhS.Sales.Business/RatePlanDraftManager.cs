using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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

		public CountryChanges GetCountryChanges(int customerId)
		{
			Changes draft = GetDraft(SalePriceListOwnerType.Customer, customerId);
			if (draft != null)
				return draft.CountryChanges;
			return null;
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

				IEnumerable<DraftNewCountry> existingCountries = (existingChanges.CountryChanges != null) ? existingChanges.CountryChanges.NewCountries : null;
				IEnumerable<DraftNewCountry> newCountries = (newChanges.CountryChanges != null) ? newChanges.CountryChanges.NewCountries : null;
				IEnumerable<int> removedCountryIds = GetRemovedCountryIds(existingCountries, newCountries);
				allChanges.CountryChanges = newChanges.CountryChanges;
				
				allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges, removedCountryIds);
				
				return allChanges;
			});
		}

		private IEnumerable<int> GetRemovedCountryIds(IEnumerable<DraftNewCountry> existingCountries, IEnumerable<DraftNewCountry> newCountries)
		{
			if (existingCountries == null)
				return null;
			
			if (newCountries == null)
				return existingCountries.MapRecords(x => x.CountryId);
			
			IEnumerable<int> existingCountryIds = existingCountries.MapRecords(x => x.CountryId);
			IEnumerable<int> newCountryIds = newCountries.MapRecords(x => x.CountryId);
			return existingCountryIds.FindAllRecords(x => !newCountryIds.Contains(x));
		}

		private List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges, IEnumerable<int> removedCountryIds)
		{
			return Merge(existingZoneChanges, newZoneChanges, () =>
			{
				foreach (ZoneChanges existingZoneDraft in existingZoneChanges)
				{
					if (!newZoneChanges.Any(x => x.ZoneId == existingZoneDraft.ZoneId))
						newZoneChanges.Add(existingZoneDraft);
				}
				if (removedCountryIds != null)
					return newZoneChanges.FindAll(x => !removedCountryIds.Contains(x.CountryId));
				else
					return newZoneChanges;
			});
		}

		private T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
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
						zoneDraft.NewRates = GetChangedOtherRates(zoneDraft.NewRates);
				}

				SaveDraft(ownerType, ownerId, draft);
			}
		}

		private IEnumerable<DraftRateToChange> GetChangedOtherRates(IEnumerable<DraftRateToChange> changedRates)
		{
			var changedOtherRates = new List<DraftRateToChange>();
			if (changedRates != null)
			{
				foreach (DraftRateToChange changedRate in changedRates)
				{
					if (changedRate.RateTypeId.HasValue)
						changedOtherRates.Add(changedRate);
				}
			}
			return changedOtherRates.Count > 0 ? changedOtherRates : null;
        }
    }

    

	public class SaveCountryChangesInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }
		public int OwnerId { get; set; }
		public CountryChanges CountryChanges { get; set; }
	}
}
