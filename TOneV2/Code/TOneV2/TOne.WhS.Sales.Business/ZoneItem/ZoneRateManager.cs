using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business
{
	public class ZoneRateManager
	{
		#region Fields

		private SalePriceListOwnerType _ownerType;
		private int _ownerId;
		private int? _sellingProductId;
		private DateTime _effectiveOn;

		private IEnumerable<DraftRateToChange> _newRates;
		private IEnumerable<DraftRateToClose> _closedRates;

		private int _targetCurrencyId;
		private int _longPrecision;

		private SaleEntityZoneRateLocator _rateLocator;
		private SaleEntityZoneRateLocator _futureRateLocator;
		private CurrencyExchangeRateManager _exchangeRateManager;
		private SaleRateManager _saleRateManager;

		private Guid _rateTypeRuleDefinitionId;
		private IEnumerable<RateType> _rateTypes;
		private Vanrise.GenericData.Pricing.RateTypeRuleManager _rateTypeRuleManager;

		private CurrencyManager _currencyManager;
		#endregion

		#region Public Methods

		public ZoneRateManager(SalePriceListOwnerType ownerType, int ownerId, int? sellingProductId, DateTime effectiveOn, Changes changes, int targetCurrencyId, int longPrecision, SaleEntityZoneRateLocator rateLocator)
		{
			_ownerType = ownerType;
			_ownerId = ownerId;
			_sellingProductId = sellingProductId;
			_effectiveOn = effectiveOn;

			if (changes != null && changes.ZoneChanges != null)
			{
				_newRates = changes.ZoneChanges.Where(x => x.NewRates != null).SelectMany(x => x.NewRates);
				_closedRates = changes.ZoneChanges.Where(x => x.ClosedRates != null).SelectMany(x => x.ClosedRates);
			}

			_targetCurrencyId = targetCurrencyId;
			_longPrecision = longPrecision;

			_rateLocator = rateLocator;
			_futureRateLocator = new SaleEntityZoneRateLocator(new FutureSaleRateReadWithCache());
			_exchangeRateManager = new CurrencyExchangeRateManager();
			_saleRateManager = new SaleRateManager();

			_rateTypeRuleDefinitionId = new Guid("8A637067-0056-4BAE-B4D5-F80F00C0141B");
			_rateTypes = GetRateTypes();
			_rateTypeRuleManager = new Vanrise.GenericData.Pricing.RateTypeRuleManager();

			_currencyManager = new CurrencyManager();
		}

		public void SetZoneRate(ZoneItem zoneItem)
		{
			SaleEntityZoneRate rate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
				_rateLocator.GetSellingProductZoneRate(_ownerId, zoneItem.ZoneId) :
				_rateLocator.GetCustomerZoneRate(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

			if (rate != null)
			{
				if (rate.Rate != null)
				{
					zoneItem.CurrentRateId = rate.Rate.SaleRateId;
					zoneItem.CurrentRate = ConvertToCurrencyAndRound(rate.Rate);
					zoneItem.CurrentRateCurrencyId = _saleRateManager.GetCurrencyId(rate.Rate);
					zoneItem.CurrentRateCurrencySymbol = _currencyManager.GetCurrencySymbol(zoneItem.CurrentRateCurrencyId.Value);
					zoneItem.CurrentRateBED = rate.Rate.BED;
					zoneItem.CurrentRateEED = rate.Rate.EED;
					zoneItem.IsCurrentRateEditable = (rate.Source == _ownerType);
				}

				if (rate.RatesByRateType != null)
				{
					zoneItem.CurrentOtherRates = new Dictionary<int, OtherRate>();
					foreach (KeyValuePair<int, SaleRate> kvp in rate.RatesByRateType)
					{
						SalePriceListOwnerType otherRateSource;
						rate.SourcesByRateType.TryGetValue(kvp.Key, out otherRateSource);

						zoneItem.CurrentOtherRates.Add(kvp.Key, new OtherRate()
						{
							RateTypeId = kvp.Key,
							Rate = ConvertToCurrencyAndRound(kvp.Value),
							CurrencyId = _saleRateManager.GetCurrencyId(kvp.Value),
							IsRateEditable = otherRateSource == _ownerType,
							BED = kvp.Value.BED,
							EED = kvp.Value.EED
						});
					}
				}
			}

			// Don't set the future rates of future zones or of those sold in the future
			if (!zoneItem.IsFutureZone && (!zoneItem.CountryBED.HasValue || zoneItem.CountryBED.Value <= DateTime.Today))
				SetFutureRates(zoneItem);

			SetZoneRateChanges(zoneItem);
			SetZoneRateTypes(zoneItem);
		}

		#endregion

		#region Private Methods

		private IEnumerable<RateType> GetRateTypes()
		{
			var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();
			IEnumerable<Vanrise.Entities.RateTypeInfo> rateTypeInfoEntities = rateTypeManager.GetAllRateTypes();
			return (rateTypeInfoEntities != null) ? rateTypeInfoEntities.MapRecords(x => new RateType() { RateTypeId = x.RateTypeId, Name = x.Name }) : _rateTypes = new List<RateType>();
		}

		private void SetFutureRates(ZoneItem zoneItem)
		{
			SaleEntityZoneRate futureRate = (_ownerType == SalePriceListOwnerType.SellingProduct) ?
				_futureRateLocator.GetSellingProductZoneRate(_ownerId, zoneItem.ZoneId) :
				_futureRateLocator.GetCustomerZoneRate(_ownerId, (int)_sellingProductId, zoneItem.ZoneId);

			if (futureRate != null)
			{
				if (futureRate.Rate != null && futureRate.Rate.BED.Date > _effectiveOn.Date)
				{
					zoneItem.FutureNormalRate = new FutureRate()
					{
						RateTypeId = futureRate.Rate.RateTypeId,
						Rate = ConvertToCurrencyAndRound(futureRate.Rate),
						IsRateEditable = futureRate.Source == _ownerType,
						BED = futureRate.Rate.BED,
						EED = futureRate.Rate.EED,
						RateChange = futureRate.Rate.RateChange,
					};
				}

				if (futureRate.RatesByRateType != null)
				{
					zoneItem.FutureOtherRates = new Dictionary<int, FutureRate>();
					foreach (KeyValuePair<int, SaleRate> kvp in futureRate.RatesByRateType)
					{
						SalePriceListOwnerType fututreOtherRateSource;
						futureRate.SourcesByRateType.TryGetValue(kvp.Key, out fututreOtherRateSource);

						if (kvp.Value.BED.Date > _effectiveOn.Date)
						{
							zoneItem.FutureOtherRates.Add(kvp.Key, new FutureRate()
							{
								RateTypeId = kvp.Value.RateTypeId,
								Rate = ConvertToCurrencyAndRound(kvp.Value),
								IsRateEditable = fututreOtherRateSource == _ownerType,
								BED = kvp.Value.BED,
								EED = kvp.Value.EED
							});
						}
					}
				}
			}
		}

		private void SetZoneRateChanges(ZoneItem zoneItem)
		{
			DraftRateToChange newNormalRate = null;
			IEnumerable<DraftRateToChange> newOtherRates = null;

			DraftRateToChange draftNormalRate = null;
			IEnumerable<DraftRateToChange> draftOtherRates = null;

			var newRates = new List<DraftRateToChange>();

			if (_newRates != null)
			{
				draftNormalRate = _newRates.FindRecord(x => x.ZoneId == zoneItem.ZoneId && !x.RateTypeId.HasValue);
				draftOtherRates = _newRates.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId && x.RateTypeId.HasValue);
			}

			if (zoneItem.NewRates != null)
			{
				newNormalRate = zoneItem.NewRates.FindRecord(x => !x.RateTypeId.HasValue);
				newOtherRates = zoneItem.NewRates.FindAllRecords(x => x.RateTypeId.HasValue);
			}

			if (newNormalRate != null)
			{
				newRates.Add(newNormalRate);
			}
			else if (draftNormalRate != null)
			{
				newRates.Add(draftNormalRate);
			}

			if (newOtherRates != null)
			{
				newRates.AddRange(newOtherRates);
			}

			if (draftOtherRates != null)
			{
				foreach (var draftOtherRate in draftOtherRates)
				{
					if (!newRates.Any(item => item.RateTypeId == draftOtherRate.RateTypeId))
						newRates.Add(draftOtherRate);
				}
			}

			zoneItem.NewRates = newRates;

			if (zoneItem.NewRates != null)
			{
				foreach (var newRate in zoneItem.NewRates)
				{
					newRate.Rate = decimal.Round(newRate.Rate, _longPrecision);
				}
			}

			DraftRateToClose closedNormalRate = null;
			if (zoneItem.ClosedRates != null)
				closedNormalRate = zoneItem.ClosedRates.FindRecord(x => !x.RateTypeId.HasValue);

			if (closedNormalRate != null)
			{
				var closedRates = new List<DraftRateToClose>();
				closedRates.Add(closedNormalRate);

				IEnumerable<DraftRateToClose> draftClosedOtherRates = _closedRates.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId && x.RateTypeId.HasValue);
				if (draftClosedOtherRates != null)
					closedRates.AddRange(draftClosedOtherRates);

				zoneItem.ClosedRates = closedRates;
				zoneItem.CurrentRateNewEED = closedNormalRate.EED;
			}
			else
			{
				zoneItem.ClosedRates = _closedRates.FindAllRecords(x => x.ZoneId == zoneItem.ZoneId);

				DraftRateToClose closedRate = zoneItem.ClosedRates.FindRecord(x => !x.RateTypeId.HasValue);
				if (closedRate != null)
					zoneItem.CurrentRateNewEED = closedRate.EED;
			}
		}

		private void SetZoneRateTypes(ZoneItem zoneItem)
		{
			if (_ownerType == SalePriceListOwnerType.SellingProduct)
				zoneItem.RateTypes = _rateTypes.ToList();
			else
			{
				Vanrise.GenericData.Entities.GenericRuleTarget target = GetTarget(zoneItem.ZoneId, DateTime.Now);
				IEnumerable<int> rateTypeIds = _rateTypeRuleManager.GetRateTypes(_rateTypeRuleDefinitionId, target);
				if (rateTypeIds != null)
					zoneItem.RateTypes = _rateTypes.FindAllRecords(x => rateTypeIds.Contains(x.RateTypeId)).ToList();
			}
		}

		private Vanrise.GenericData.Entities.GenericRuleTarget GetTarget(long zoneId, DateTime? effectiveDate)
		{
			var target = new Vanrise.GenericData.Entities.GenericRuleTarget();
			target.TargetFieldValues = new Dictionary<string, object>();
			target.TargetFieldValues.Add("CustomerId", _ownerId);
			target.TargetFieldValues.Add("SaleZoneId", zoneId);
			target.EffectiveOn = effectiveDate;
			return target;
		}

		private decimal ConvertToCurrencyAndRound(SaleRate saleRate)
		{
			return UtilitiesManager.ConvertToCurrencyAndRound(saleRate.Rate, _saleRateManager.GetCurrencyId(saleRate), _targetCurrencyId, _effectiveOn, _longPrecision, _exchangeRateManager);
		}

		#endregion
	}
}
