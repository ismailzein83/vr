using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public sealed class UpdateImportedBEDs : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierCode>> ExistingCodeEntities { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SupplierZoneService>> ExistingZoneServiceEntities { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZone>> ImportedZones { get; set; }

        #endregion
        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SupplierCode> existingCodeEntities = this.ExistingCodeEntities.Get(context);
            IEnumerable<SupplierRate> existingRateEntities = this.ExistingRateEntities.Get(context);
            IEnumerable<SupplierZoneService> existingZoneServiceEntities = this.ExistingZoneServiceEntities.Get(context);

            IEnumerable<ImportedCode> importedCodes = this.ImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRates = this.ImportedRates.Get(context);
            IEnumerable<ImportedZone> importedZones = this.ImportedZones.Get(context);

            var supplierZoneManager = new SupplierZoneManager();

            #region Codes

            Dictionary<string, List<SupplierCode>> existingCodesByCode = StructureExistingCodesByCode(existingCodeEntities);

            foreach (var importedCode in importedCodes)
            {
                List<SupplierCode> importedCodeExistingEntities = new List<SupplierCode>();

                if (existingCodesByCode.TryGetValue(importedCode.Code, out importedCodeExistingEntities))
                {
                    var matchedCode = importedCodeExistingEntities.FindRecord(item => item.IsInTimeRange(importedCode.BED));
                    if (matchedCode != null && supplierZoneManager.GetSupplierZoneName(matchedCode.ZoneId) == importedCode.ZoneName)
                        importedCode.BED = matchedCode.BED;
                }
            }
            #endregion

            #region Rates

            Dictionary<string, List<SupplierRate>> existingNormalRates = new Dictionary<string, List<SupplierRate>>();
            Dictionary<string, Dictionary<int, List<SupplierRate>>> existingOtherRates = new Dictionary<string, Dictionary<int, List<SupplierRate>>>();
            StructureExistingRatesByZoneName(existingRateEntities, existingNormalRates, existingOtherRates);

            foreach (var importedRate in importedRates)
            {
                List<SupplierRate> importedRateExistingEntities = new List<SupplierRate>();

                if (!importedRate.RateTypeId.HasValue)
                {
                    if (!existingNormalRates.TryGetValue(importedRate.ZoneName, out importedRateExistingEntities))
                        break;
                }

                else
                {
                    Dictionary<int, List<SupplierRate>> importedOtherRatesExistingEntities = new Dictionary<int, List<SupplierRate>>();
                    if (!existingOtherRates.TryGetValue(importedRate.ZoneName, out importedOtherRatesExistingEntities))
                        break;
                    if (!importedOtherRatesExistingEntities.TryGetValue(importedRate.RateTypeId.Value, out importedRateExistingEntities))
                        break;
                }

                var matchedRate = importedRateExistingEntities.FindRecord(item => item.IsInTimeRange(importedRate.BED));
                if (matchedRate != null && matchedRate.Rate == importedRate.Rate)
                    importedRate.BED = matchedRate.BED;
            }

            #endregion

            #region Services

            Dictionary<string, List<SupplierZoneService>> existingZoneServicesByZoneName = StructureExistingZoneServicesByZoneName(existingZoneServiceEntities);

            foreach (var importedZone in importedZones)
            {
                if (importedZone.ImportedZoneServiceGroup == null)
                    continue;
                List<SupplierZoneService> importedZoneExistingEntities = new List<SupplierZoneService>();

                if (existingZoneServicesByZoneName.TryGetValue(importedZone.ZoneName, out importedZoneExistingEntities))
                {
                    var matchedZoneService = importedZoneExistingEntities.FindRecord(item => item.IsInTimeRange(importedZone.ImportedZoneServiceGroup.BED));
                    if (matchedZoneService != null && SameServiceIds(matchedZoneService.ReceivedServices, importedZone.ImportedZoneServiceGroup.ServiceIds))
                    {
                        importedZone.ImportedZoneServiceGroup.BED = matchedZoneService.BED;
                    }
                }
            }

            #endregion
        }

        private Dictionary<string, List<SupplierCode>> StructureExistingCodesByCode(IEnumerable<SupplierCode> existingCodeEntities)
        {
            Dictionary<string, List<SupplierCode>> existingCodesByCode = new Dictionary<string, List<SupplierCode>>();

            foreach (var code in existingCodeEntities)
            {
                var codeExistingEntities = new List<SupplierCode>();

                if (!existingCodesByCode.TryGetValue(code.Code, out codeExistingEntities))
                {
                    codeExistingEntities = new List<SupplierCode>();
                    existingCodesByCode.Add(code.Code, codeExistingEntities);
                }
                codeExistingEntities.Add(code);
            }
            return existingCodesByCode;
        }

        private void StructureExistingRatesByZoneName(IEnumerable<SupplierRate> existingRateEntities, Dictionary<string, List<SupplierRate>> existingNormalRatesByZoneName, Dictionary<string, Dictionary<int, List<SupplierRate>>> existingOtherRatesByZoneName)
        {
            var supplierZoneManager = new SupplierZoneManager();

            foreach (var rate in existingRateEntities)
            {
                var zoneName = supplierZoneManager.GetSupplierZoneName(rate.ZoneId);
                if (!rate.RateTypeId.HasValue)
                {
                    List<SupplierRate> rateExistingEntities = new List<SupplierRate>();
                    if (!existingNormalRatesByZoneName.TryGetValue(zoneName, out rateExistingEntities))
                    {
                        rateExistingEntities = new List<SupplierRate>();
                        existingNormalRatesByZoneName.Add(zoneName, rateExistingEntities);
                    }
                    rateExistingEntities.Add(rate);
                }
                else
                {
                    Dictionary<int, List<SupplierRate>> rateExistingEntities = new Dictionary<int, List<SupplierRate>>();
                    if (!existingOtherRatesByZoneName.TryGetValue(zoneName, out rateExistingEntities))
                    {
                        rateExistingEntities = new Dictionary<int, List<SupplierRate>>();
                        existingOtherRatesByZoneName.Add(zoneName, rateExistingEntities);
                    }
                    List<SupplierRate> matchedRates = new List<SupplierRate>();
                    if (!rateExistingEntities.TryGetValue(rate.RateTypeId.Value, out matchedRates))
                    {
                        matchedRates = new List<SupplierRate>();
                        rateExistingEntities.Add(rate.RateTypeId.Value, matchedRates);
                    }
                    matchedRates.Add(rate);
                }
            }
        }

        private Dictionary<string, List<SupplierZoneService>> StructureExistingZoneServicesByZoneName(IEnumerable<SupplierZoneService> existingZoneServiceEntities)
        {
            var supplierZoneManager = new SupplierZoneManager();
            Dictionary<string, List<SupplierZoneService>> existingZoneServicesByZoneName = new Dictionary<string, List<SupplierZoneService>>();

            foreach (var zoneService in existingZoneServiceEntities)
            {
                var zoneName = supplierZoneManager.GetSupplierZoneName(zoneService.ZoneId);
                List<SupplierZoneService> zoneServiceExistingEntities = new List<SupplierZoneService>();

                if (!existingZoneServicesByZoneName.TryGetValue(zoneName, out zoneServiceExistingEntities))
                {
                    zoneServiceExistingEntities = new List<SupplierZoneService>();
                    existingZoneServicesByZoneName.Add(zoneName, zoneServiceExistingEntities);
                }
                zoneServiceExistingEntities.Add(zoneService);
            }
            return existingZoneServicesByZoneName;
        }

        private bool SameServiceIds(List<ZoneService> zoneServices, List<int> serviceIds)
        {
            if (!(zoneServices.Count == serviceIds.Count))
                return false;

            foreach (ZoneService zoneService in zoneServices)
            {
                if (!serviceIds.Contains(zoneService.ServiceId))
                    return false;
            }

            return true;
        }
    }
}
