﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class ImportBulkAction : BulkActionType
	{
		#region Fields / Constructors

		private RoutingProductManager _routingProductManager = new RoutingProductManager();

		private ImportBulkActionValidationCacheManager _cacheManager;

		public ImportBulkAction()
		{
			_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<ImportBulkActionValidationCacheManager>();
		}

		#endregion

		public long FileId { get; set; }

		public bool HeaderRowExists { get; set; }

		public string DateTimeFormat { get; set; }

		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public Guid CacheObjectName { get; set; }

		#region Bulk Action Members

		public override Guid ConfigId
		{
			get { return new Guid("1136DAC5-A1EE-4C72-B12C-05FF513CE3D3"); }
		}

		public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
		{
			return UtilitiesManager.IsActionApplicableToCountry(context, this.IsApplicableToZone);
		}

		public override void ValidateZone(IZoneValidationContext context)
		{
			if (context.ValidationResult == null)
			{
				var validationResult = new ImportBulkActionValidationResult();

				ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
				if (cachedValidationData.FileIsEmpty)
				{
					validationResult.InvalidDataExists = true;
					validationResult.ErrorMessage = "Imported file is empty";
				}
				if (cachedValidationData.InvalidDataByRowIndex != null && cachedValidationData.InvalidDataByRowIndex.Values != null && cachedValidationData.InvalidDataByRowIndex.Values.Count > 0)
				{
					validationResult.InvalidDataExists = true;

					foreach (InvalidImportedRow invalidImportedRow in cachedValidationData.InvalidDataByRowIndex.Values)
					{
						validationResult.InvalidImportedRows.Add(invalidImportedRow);

						if (invalidImportedRow.ZoneId.HasValue && invalidImportedRow.Status != ImportedRowStatus.OnlyNormalRateValid)
						{
							validationResult.ExcludedZoneIds.Add(invalidImportedRow.ZoneId.Value);
						}
					}
				}

				context.ValidationResult = validationResult;
			}
		}

		public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			return cachedValidationData.ApplicableZoneIds.Contains(context.SaleZone.SaleZoneId);
		}

		public override Dictionary<int, DateTime> PreApplyBulkActionToZoneItem()
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			return cachedValidationData.AdditionalCountryBEDsByCountryId;
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			IEnumerable<DraftRateToChange> zoneDraftNewRates = (context.ZoneDraft != null) ? context.ZoneDraft.NewRates : null;
			DateTime? newOtherRateBED;
			context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate, out newOtherRateBED);
			var zoneNewRoutingProduct = GetZoneNewRoutingProduct(context.ZoneItem.ZoneId);

			if (zoneNewRoutingProduct != null)
			{
				if (zoneNewRoutingProduct.ZoneRoutingProductId == context.ZoneItem.CurrentRoutingProductId)
				{
					context.ZoneItem.ResetRoutingProduct = null;
					context.ZoneItem.NewRoutingProduct = null;
					context.ZoneItem.EffectiveRoutingProductId = null;
					context.ZoneItem.EffectiveRoutingProductName = null;
					context.ZoneItem.EffectiveServiceIds = null;

				}
				else
				{
					context.ZoneItem.ResetRoutingProduct = null;
					context.ZoneItem.NewRoutingProduct = zoneNewRoutingProduct;
					context.ZoneItem.EffectiveRoutingProductId = zoneNewRoutingProduct.ZoneRoutingProductId;
					context.ZoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(zoneNewRoutingProduct.ZoneRoutingProductId);
					context.ZoneItem.EffectiveServiceIds = _routingProductManager.GetZoneServiceIds(zoneNewRoutingProduct.ZoneRoutingProductId, context.ZoneItem.ZoneId);
				}
			}
			else if (context.ZoneDraft != null && context.ZoneDraft.NewRoutingProduct != null)
			{
				context.ZoneItem.ResetRoutingProduct = context.ZoneDraft.RoutingProductChange;
				context.ZoneItem.NewRoutingProduct = context.ZoneDraft.NewRoutingProduct;
				context.ZoneItem.EffectiveRoutingProductId = context.ZoneDraft.NewRoutingProduct.ZoneRoutingProductId;
				context.ZoneItem.EffectiveRoutingProductName = _routingProductManager.GetRoutingProductName(context.ZoneDraft.NewRoutingProduct.ZoneRoutingProductId);
				context.ZoneItem.EffectiveServiceIds = _routingProductManager.GetZoneServiceIds(context.ZoneDraft.NewRoutingProduct.ZoneRoutingProductId, context.ZoneItem.ZoneId);
			}
			if (newOtherRateBED != null)
				context.ZoneItem.NewOtherRateBED = newOtherRateBED;
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			var zoneItem = context.GetZoneItem(context.ZoneDraft.ZoneId);
			var zoneNewRoutingProduct = GetZoneNewRoutingProduct(context.ZoneDraft.ZoneId);
			if (zoneNewRoutingProduct != null)
			{
				if (zoneNewRoutingProduct.ZoneRoutingProductId == zoneItem.CurrentRoutingProductId)
				{
					context.ZoneDraft.NewRoutingProduct = null;
					context.ZoneDraft.RoutingProductChange = null;
				}
				else
				{
					context.ZoneDraft.NewRoutingProduct = zoneNewRoutingProduct;
					context.ZoneDraft.RoutingProductChange = null;
				}
			}

			DateTime? newOtherRateBED;
			context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate, out newOtherRateBED);
			if (newOtherRateBED != null)
				context.ZoneDraft.NewOtherRateBED = newOtherRateBED;
		}

		#endregion

		#region Private Methods


		private DraftNewSaleZoneRoutingProduct GetZoneNewRoutingProduct(long zoneId)
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			ImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);
			if (importedRow == null || importedRow.RoutingProductId == null)
				return null;

			return new DraftNewSaleZoneRoutingProduct() { ZoneId = zoneId, ZoneRoutingProductId = importedRow.RoutingProductId.Value, BED = Convert.ToDateTime(importedRow.EffectiveDate), ApplyNewNormalRateBED = true };
		}

		private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates, Func<decimal, decimal> getRoundedRate, out DateTime? newOtherRateBED)
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			ImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);
			newOtherRateBED = null;

			if (importedRow == null)
				return zoneDraftNewRates;

			var newRates = new List<DraftRateToChange>();

			DraftRateToChange newNormalRate = null;

			if (!string.IsNullOrEmpty(importedRow.Rate))
			{
				newNormalRate = new DraftRateToChange()
				{
					ZoneId = zoneId,
					RateTypeId = null,
					Rate = getRoundedRate(Convert.ToDecimal(importedRow.Rate)),
					BED = Convert.ToDateTime(importedRow.EffectiveDate)
				};
			}
			else if (zoneDraftNewRates != null)
				newNormalRate = zoneDraftNewRates.FindRecord(item => !item.RateTypeId.HasValue);

			if (newNormalRate != null)
			{
				newOtherRateBED = newNormalRate.BED;
				newRates.Add(newNormalRate);
			}

			foreach (var otherRate in importedRow.OtherRates)
			{
				if (!string.IsNullOrEmpty(otherRate.Value))
				{
					if (!newOtherRateBED.HasValue)
						newOtherRateBED = Convert.ToDateTime(importedRow.EffectiveDate);
					var newOtherRate = new DraftRateToChange()
					{
						ZoneId = zoneId,
						RateTypeId = otherRate.TypeId,
						Rate = getRoundedRate(Convert.ToDecimal(otherRate.Value)),
						BED = newOtherRateBED.Value
					};
					newRates.Add(newOtherRate);
				}
			}

			if (zoneDraftNewRates != null)
			{
				foreach (DraftRateToChange newRate in zoneDraftNewRates)
				{
					if (newRate.RateTypeId.HasValue && !newRates.Any(item => item.RateTypeId == newRate.RateTypeId))
					{
						if (!newOtherRateBED.HasValue)
							newOtherRateBED = newRate.BED;

						newRate.BED = newOtherRateBED.Value;
						newRates.Add(newRate);
					}
				}
			}
			return newRates;
		}

		private ImportedDataValidationResult GetOrCreateObject()
		{
			return _cacheManager.GetOrCreateObject(CacheObjectName, () =>
			{
				return new RatePlanManager().ValidateImportedData(new ImportedDataValidationInput()
				{
					FileId = FileId,
					HeaderRowExists = HeaderRowExists,
					DateTimeFormat = DateTimeFormat,
					OwnerType = OwnerType,
					OwnerId = OwnerId
				});
			});
		}

		#endregion
	}
}
