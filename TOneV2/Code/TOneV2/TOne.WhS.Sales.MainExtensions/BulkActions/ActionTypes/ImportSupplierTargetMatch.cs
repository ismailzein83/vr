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
    public class ImportSupplierTargetMatch : BulkActionType
    {
        private ImportBulkActionValidationCacheManager _cacheManager;

        public ImportSupplierTargetMatch()
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
        public override bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context)
        {
            return UtilitiesManager.IsActionApplicableToCountry(context, this.IsApplicableToZone);
        }
        public override bool IsApplicableToZone(IActionApplicableToZoneContext context)
        {
            ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
            return cachedValidationData.ApplicableZoneIds.Contains(context.SaleZone.SaleZoneId);
        }
        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            IEnumerable<DraftRateToChange> zoneDraftNewRates = (context.ZoneDraft != null) ? context.ZoneDraft.NewRates : null;
           
            context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates, context.GetRoundedRate);
          
        }
        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates, context.GetRoundedRate);
        }
        public override Dictionary<int, DateTime> PreApplyBulkActionToZoneItem()
        {
            ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
            return cachedValidationData.AdditionalCountryBEDsByCountryId;
        }
        #endregion

        private ImportedDataValidationResult GetOrCreateObject()
        {
            return _cacheManager.GetOrCreateObject(CacheObjectName, () =>
            {
                return new RatePlanManager().ValidateTargetMatchImportedData(new TargetMatchImportedDataInput()
                {
                    FileId = FileId,
                    HeaderRowExists = HeaderRowExists,
                    OwnerId = OwnerId
                });
            });
        }

        private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates, Func<decimal, decimal> getRoundedRate)
        {
            ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
            ImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);
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
