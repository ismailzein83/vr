using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
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

		public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
		{
			IEnumerable<DraftRateToChange> zoneDraftNewRates = (context.ZoneDraft != null) ? context.ZoneDraft.NewRates : null;
			context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate, context.ZoneCurrentRate);
		}

		public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
		{
			context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate,context.ZoneCurrentRate);
		}

		#endregion

		#region Private Methods

		private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates, Func<decimal, decimal> getRoundedRate, SaleEntityZoneRate zoneCurrentRate)
		{
			ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
			ImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);

			if (importedRow == null)
				return zoneDraftNewRates;

			var newRates = new List<DraftRateToChange>();

			if (zoneDraftNewRates != null)
				newRates.AddRange(zoneDraftNewRates);

			if (importedRow.OtherRates != null && importedRow.OtherRates.Count > 0)
			{
				foreach (var otherRate in importedRow.OtherRates)
				{
					if (!string.IsNullOrEmpty(otherRate.Value) && !newRates.Any(item => item.RateTypeId == otherRate.TypeId))
					{
						var newOtherRate = new DraftRateToChange()
						{
							ZoneId = zoneId,
							RateTypeId = otherRate.TypeId,
							Rate = getRoundedRate(Convert.ToDecimal(otherRate.Value)),
							BED = Convert.ToDateTime(importedRow.EffectiveDate)//Remove it
						};
						newRates.Add(newOtherRate);
					}
				}
			}

			if (!newRates.Any(item => item.RateTypeId == null))
			{
				if (!string.IsNullOrEmpty(importedRow.Rate))
				{
					var newNormalRate = new DraftRateToChange()
					{
						ZoneId = zoneId,
						RateTypeId = null,
						Rate = getRoundedRate(Convert.ToDecimal(importedRow.Rate)),
						BED = Convert.ToDateTime(importedRow.EffectiveDate)
					};
					newRates.Add(newNormalRate);
				}
				else if (newRates.Count > 0)
				{
					var newNormalRate = new DraftRateToChange()
					{
						ZoneId = zoneId,
						RateTypeId = null,
						Rate = zoneCurrentRate.Rate.Rate,
						BED = Convert.ToDateTime(importedRow.EffectiveDate)
					};
					newRates.Add(newNormalRate);
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
