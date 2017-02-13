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
        private ImportBulkActionValidationCacheManager _cacheManager;

        public override Guid ConfigId
        {
            get { return new Guid("1136DAC5-A1EE-4C72-B12C-05FF513CE3D3"); }
        }

        public long FileId { get; set; }

        public bool HeaderRowExists { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public Guid CacheObjectName { get; set; }

        public ImportBulkAction()
        {
            _cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<ImportBulkActionValidationCacheManager>();
        }

        public override void ValidateZone(IZoneValidationContext context)
        {
            if (context.ValidationResult == null)
            {
                var validationResult = new ImportBulkActionValidationResult();

                ImportedDataValidationResult cachedValidationData = GetOrCreateObject();

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
            return cachedValidationData.ValidDataByZoneId.ContainsKey(context.SaleZone.SaleZoneId);
        }

        public override void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context)
        {
            IEnumerable<DraftRateToChange> zoneDraftNewRates = (context.ZoneDraft != null) ? context.ZoneDraft.NewRates : null;
            context.ZoneItem.NewRates = GetZoneItemNewRates(context.ZoneItem.ZoneId, zoneDraftNewRates);
        }

        public override void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context)
        {
            context.ZoneDraft.NewRates = GetZoneItemNewRates(context.ZoneDraft.ZoneId, context.ZoneDraft.NewRates);
        }

        private IEnumerable<DraftRateToChange> GetZoneItemNewRates(long zoneId, IEnumerable<DraftRateToChange> zoneDraftNewRates)
        {
            ImportedDataValidationResult cachedValidationData = GetOrCreateObject();
            ImportedRow importedRow = cachedValidationData.ValidDataByZoneId.GetRecord(zoneId);

            var newRates = new List<DraftRateToChange>();

            if (zoneDraftNewRates != null)
            {
                foreach (DraftRateToChange newRate in zoneDraftNewRates)
                {
                    if (newRate.RateTypeId.HasValue)
                        newRates.Add(newRate);
                }
            }

            var newNormalRate = new DraftRateToChange()
            {
                ZoneId = zoneId,
                RateTypeId = null,
                Rate = Convert.ToDecimal(importedRow.Rate),
                BED = Convert.ToDateTime(importedRow.EffectiveDate)
            };

            newRates.Add(newNormalRate);
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
                    OwnerType = OwnerType,
                    OwnerId = OwnerId
                });
            });
        }
    }
}
