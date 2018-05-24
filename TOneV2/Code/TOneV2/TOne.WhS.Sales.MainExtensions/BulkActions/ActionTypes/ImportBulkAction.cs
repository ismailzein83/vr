using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Caching;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
	public class ImportBulkAction : BulkActionType
	{
		#region Fields / Constructors

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

						if (invalidImportedRow.ZoneId.HasValue)
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

		public override Dictionary<int,DateTime> PreApplyBulkActionToZoneItem()
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			return cachedValidationData.AdditionalCountryBEDsByCountryId;
		}

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			IEnumerable<DraftRateToChange> zoneDraftNewRates = (context.ZoneDraft != null) ? context.ZoneDraft.NewRates : null;
			DateTime? newOtherRateBED;
			context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate, out newOtherRateBED);
			if (newOtherRateBED != null)
				context.ZoneItem.NewOtherRateBED = newOtherRateBED;
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			DateTime? newOtherRateBED;
			context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate, out newOtherRateBED);
			if (newOtherRateBED != null)
				context.ZoneDraft.NewOtherRateBED = newOtherRateBED;
		}

		#endregion

		#region Private Methods

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
