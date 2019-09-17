﻿using System;
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

        public override void ValidateZone(IZoneValidationContext context)
        {
            if (context.ValidationResult == null)
            {
                var validationResult = new CustomerTargetMatchImportBulkActionValidationResult();
                if (context.CustomObject == null)
                {
                    context.CustomObject = new RatePlanManager().ValidateTargetMatchImportedData(new TargetMatchImportedDataInput()
                    {
                        FileId = FileId,
                        HeaderRowExists = HeaderRowExists,
                        OwnerId = OwnerId,
                        RateCalculationMethod = RateCalculationMethod,
                        GetZoneItem = context.GetContextZoneItem,
                        GetCostCalculationMethodIndex = context.GetCostCalculationMethodIndex
                    });
                }
                var customerTargetMatchImportedDataValidationResult = context.CustomObject as CustomerTargetMatchImportedDataValidationResult;
                if (customerTargetMatchImportedDataValidationResult.FileIsEmpty)
                {
                    validationResult.InvalidDataExists = true;
                    validationResult.ErrorMessage = "Imported file is empty";
                }
                if (customerTargetMatchImportedDataValidationResult.InvalidDataByRowIndex != null && customerTargetMatchImportedDataValidationResult.InvalidDataByRowIndex.Values != null && customerTargetMatchImportedDataValidationResult.InvalidDataByRowIndex.Values.Count > 0)
                {
                    validationResult.InvalidDataExists = true;

                    foreach (CustomerTargetMatchInvalidImportedRow invalidImportedRow in customerTargetMatchImportedDataValidationResult.InvalidDataByRowIndex.Values)
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
            if (context.CustomObject == null)
            {
                context.CustomObject = new RatePlanManager().ValidateTargetMatchImportedData(new TargetMatchImportedDataInput()
                {
                    FileId = FileId,
                    HeaderRowExists = HeaderRowExists,
                    OwnerId = OwnerId,
                    RateCalculationMethod = RateCalculationMethod,
                    GetZoneItem = context.GetContextZoneItem,
                    GetCostCalculationMethodIndex = context.GetCostCalculationMethodIndex
                });
            }
            var customerTargetMatchImportedDataValidationResult = context.CustomObject as CustomerTargetMatchImportedDataValidationResult;
            return customerTargetMatchImportedDataValidationResult.ApplicableZoneIds.Contains(context.SaleZone.SaleZoneId);
        }
        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            IEnumerable<DraftRateToChange> zoneDraftNewRates = context.ZoneDraft?.NewRates;
            string targetVolume;
            context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate, context.GetContextZoneItem, context.GetCostCalculationMethodIndex,context.CustomObject, out targetVolume);
            context.ZoneItem.TargetVolume = targetVolume;
        }
        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            string targetVolume;
            context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate, context.GetZoneItem, context.GetCostCalculationMethodIndex,context.CustomObject, out targetVolume);
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

        private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates, Func<decimal, decimal> getRoundedRate, Func<long, ZoneItem> getZoneItem, Func<Guid, int?> getCostCalculationMethodIndex,object customObject, out string targetVolume)
        {
            if(customObject == null)
            {
                customObject = new RatePlanManager().ValidateTargetMatchImportedData(new TargetMatchImportedDataInput()
                {
                    FileId = FileId,
                    HeaderRowExists = HeaderRowExists,
                    OwnerId = OwnerId,
                    RateCalculationMethod = RateCalculationMethod,
                    GetZoneItem = getZoneItem,
                    GetCostCalculationMethodIndex = getCostCalculationMethodIndex
                });
            }

            var customerTargetMatchImportedDataValidationResult = customObject as CustomerTargetMatchImportedDataValidationResult;
            CustomerTargetMatchImportedRow importedRow = customerTargetMatchImportedDataValidationResult.ValidDataByZoneId.GetRecord(zoneId);
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
                    BED = DateTime.Today
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
