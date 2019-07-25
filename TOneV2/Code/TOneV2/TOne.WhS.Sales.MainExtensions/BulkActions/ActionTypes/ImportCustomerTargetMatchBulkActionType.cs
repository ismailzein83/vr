using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
    public class ImportCustomerTargetMatchBulkActionType : BulkActionType
    {
        private ImportBulkActionValidationCacheManager _cacheManager;

        public ImportCustomerTargetMatchBulkActionType()
        {
            _cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<ImportBulkActionValidationCacheManager>();
        }
        #region Bulk Action Members
        public override Guid ConfigId
        {
            get { return new Guid("E6D4013F-F618-4F42-8308-4D0C0A282B47"); }
        }
        public long FileId { get; set; }
        public RateCalculationMethod RateCalculationMethod { get; set; }

        public bool HeaderRowExists { get; set; }

        public int OwnerId { get; set; }

        public Guid CacheObjectName { get; set; }
        public override void ValidateZone(IZoneValidationContext context)
        {
            if (context.ValidationResult == null)
            {
                var validationResult = new CustomerTargetMatchImportBulkActionValidationResult();

                CustomerTargetMatchImportedDataValidationResult cachedValidationData = GetOrCreateObject(context.GetContextZoneItem, context.GetCostCalculationMethodIndex);
                if (cachedValidationData.FileIsEmpty)
                {
                    validationResult.InvalidDataExists = true;
                    validationResult.ErrorMessage = "Imported file is empty";
                }
                if (cachedValidationData.InvalidDataByRowIndex != null && cachedValidationData.InvalidDataByRowIndex.Values != null && cachedValidationData.InvalidDataByRowIndex.Values.Count > 0)
                {
                    validationResult.InvalidDataExists = true;

                    foreach (CustomerTargetMatchInvalidImportedRow invalidImportedRow in cachedValidationData.InvalidDataByRowIndex.Values)
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
        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            //return UtilitiesManager.IsActionApplicableToCountry(context, this.IsApplicableToZone);
            return true;
        }
        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            CustomerTargetMatchImportedDataValidationResult cachedValidationData = GetOrCreateObject(context.GetContextZoneItem, context.GetCostCalculationMethodIndex);
            return cachedValidationData.ApplicableZoneIds.Contains(context.SaleZone.SaleZoneId);
        }
        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            IEnumerable<DraftRateToChange> zoneDraftNewRates = context.ZoneDraft?.NewRates;
            string targetVolume;
            context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate, context.GetContextZoneItem, context.GetCostCalculationMethodIndex, out targetVolume);
            context.ZoneItem.TargetVolume = targetVolume;
        }
        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            string targetVolume;
            context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate, context.GetZoneItem, context.GetCostCalculationMethodIndex, out targetVolume);
            context.ZoneDraft.TargetVolume = targetVolume;
        }
        public override void ApplyCorrectedData(IApplyCorrectedDataContext context)
        {
            var correctedData = context.CorrectedData.CastWithValidate<CustomerTargetMatchBulkActionCorrectedData>("CustomerTargetMatchBulkActionCorrectedData");
            correctedData.IncludedZones.ThrowIfNull("CustomerTargetMatchBulkActionCorrectedData.IncludedZones");

            foreach (var includedZone in correctedData.IncludedZones)
            {
                ZoneChanges zoneDraft = context.GetZoneDraft(includedZone.ZoneId);
                AddNewNormalRate(zoneDraft, includedZone.Rate, DateTime.Today);
                zoneDraft.TargetVolume = includedZone.TargetVolume;
            }
        }

        private void AddNewNormalRate(ZoneChanges zoneDraft, decimal rate, DateTime effectiveDate)
        {
            var newRates = new List<DraftRateToChange>();

            IEnumerable<DraftRateToChange> newOtherRates = zoneDraft.NewRates?.FindAllRecords(x => x.RateTypeId.HasValue);
            if (newOtherRates != null && newOtherRates.Count() > 0)
                newRates.AddRange(newOtherRates);

            var newNormalRate = new DraftRateToChange()
            {
                ZoneId = zoneDraft.ZoneId,
                RateTypeId = null,
                Rate = rate,
                BED = effectiveDate
            };

            newRates.Add(newNormalRate);
            zoneDraft.NewRates = newRates;
        }

        #endregion

        private CustomerTargetMatchImportedDataValidationResult GetOrCreateObject(Func<long, ZoneItem> getZoneItem, Func<Guid, int?> getCostCalculationMethodIndex)
        {
            return _cacheManager.GetOrCreateObject(CacheObjectName, () =>
            {
                return new RatePlanManager().ValidateTargetMatchImportedData(new TargetMatchImportedDataInput()
                {
                    FileId = FileId,
                    HeaderRowExists = HeaderRowExists,
                    OwnerId = OwnerId,
                    RateCalculationMethod = RateCalculationMethod,
                    GetZoneItem = getZoneItem,
                    GetCostCalculationMethodIndex = getCostCalculationMethodIndex
                });
            });
        }

        private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates, Func<decimal, decimal> getRoundedRate, Func<long, ZoneItem> getZoneItem, Func<Guid, int?> getCostCalculationMethodIndex, out string targetVolume)
        {
            CustomerTargetMatchImportedDataValidationResult cachedValidationData = GetOrCreateObject(getZoneItem, getCostCalculationMethodIndex);
            CustomerTargetMatchImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);
            targetVolume = null;
            if (importedRow == null)
                return zoneDraftNewRates;

            targetVolume = importedRow.TargetVolume;

            var newRates = new List<DraftRateToChange>();

            DraftRateToChange newNormalRate = null;

            if (!string.IsNullOrEmpty(importedRow.Rate))
            {
                newNormalRate = new DraftRateToChange()
                {
                    ZoneId = zoneId,
                    RateTypeId = null,
                    Rate = getRoundedRate(Convert.ToDecimal(importedRow.Rate)),
                    BED = DateTime.Now
                };
            }
            else if (zoneDraftNewRates != null)
                newNormalRate = zoneDraftNewRates.FindRecord(item => !item.RateTypeId.HasValue);

            if (newNormalRate != null)
            {
                newRates.Add(newNormalRate);
            }

            return newRates;
        }
    }
}
