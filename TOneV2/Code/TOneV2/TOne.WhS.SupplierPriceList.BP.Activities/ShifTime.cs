using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ShifTime : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedRate>> ImportedRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ImportedZoneService>> ImportedZonesServices { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            int supplieId = SupplierId.Get(context);
            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRates = ImportedRates.Get(context);
            IEnumerable<ImportedZoneService> importedZonesServices = ImportedZonesServices.Get(context);

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(supplieId);

            VRTimeZoneManager timeZoneManager = new VRTimeZoneManager();
            VRTimeZone supplierTimeZone = timeZoneManager.GetVRTimeZone(supplier.SupplierSettings.TimeZoneId);

            foreach (var importedCode in importedCodes)
            {
                importedCode.BED = ShiftDateTime(importedCode.BED, supplierTimeZone.Settings.Offset).Value;
                importedCode.EED = ShiftDateTime(importedCode.EED, supplierTimeZone.Settings.Offset);
            }
            foreach (var importedRate in importedRates)
            {
                importedRate.BED = ShiftDateTime(importedRate.BED, supplierTimeZone.Settings.Offset).Value;
                importedRate.EED = ShiftDateTime(importedRate.EED, supplierTimeZone.Settings.Offset);
            }
            foreach (var importedZoneService in importedZonesServices)
            {
                importedZoneService.BED = ShiftDateTime(importedZoneService.BED, supplierTimeZone.Settings.Offset).Value;
                importedZoneService.EED = ShiftDateTime(importedZoneService.EED, supplierTimeZone.Settings.Offset);
            }
        }

        private DateTime? ShiftDateTime(DateTime? date, TimeSpan offSet)
        {
            if (!date.HasValue) return null;
            return date.Value.Subtract(offSet);
        }

    }
}
