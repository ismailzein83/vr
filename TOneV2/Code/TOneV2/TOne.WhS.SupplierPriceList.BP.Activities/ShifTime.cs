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

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        public OutArgument<DateTime> ShiftedMinimumDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int supplieId = SupplierId.Get(context);
            IEnumerable<ImportedCode> importedCodes = ImportedCodes.Get(context);
            IEnumerable<ImportedRate> importedRates = ImportedRates.Get(context);
            IEnumerable<ImportedZoneService> importedZonesServices = ImportedZonesServices.Get(context);

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            VRTimeZoneManager timeZoneManager = new VRTimeZoneManager();
            VRTimeZone supplierTimeZone = timeZoneManager.GetVRTimeZone(carrierAccountManager.GetSupplierTimeZoneId(supplieId));

            supplierTimeZone.ThrowIfNull("SupplierTimeZone", supplieId);
            supplierTimeZone.Settings.ThrowIfNull("SupplierTimeZoneSettings", supplieId);

            foreach (var importedCode in importedCodes)
            {
                if (importedCode.BED == DateTime.MinValue)
                {
                    throw new Vanrise.Entities.VRBusinessException(string.Format("BED of imported code '{0}' is mapped to an invalid column", importedCode.Code));
                }
                importedCode.BED = ShiftDateTime(importedCode.BED, supplierTimeZone.Settings.Offset).Value;
                importedCode.EED = ShiftDateTime(importedCode.EED, supplierTimeZone.Settings.Offset);
            }
            foreach (var importedRate in importedRates)
            {
                if (importedRate.BED == DateTime.MinValue)
                {
                    throw new Vanrise.Entities.VRBusinessException(string.Format("BED of imported rate for zone '{0}' mapped to an invalid column", importedRate.ZoneName));
                }
                importedRate.BED = ShiftDateTime(importedRate.BED, supplierTimeZone.Settings.Offset).Value;
                importedRate.EED = ShiftDateTime(importedRate.EED, supplierTimeZone.Settings.Offset);
            }
            foreach (var importedZoneService in importedZonesServices)
            {
                if (importedZoneService.BED == DateTime.MinValue)
                {
                    throw new Vanrise.Entities.VRBusinessException(string.Format("BED of imported service for zone '{0}' mapped to an invalid column", importedZoneService.ZoneName));
                }
                importedZoneService.BED = ShiftDateTime(importedZoneService.BED, supplierTimeZone.Settings.Offset).Value;
                importedZoneService.EED = ShiftDateTime(importedZoneService.EED, supplierTimeZone.Settings.Offset);
            }

            DateTime minimumDate = MinimumDate.Get(context);
            var shiftedDate  = ShiftDateTime(minimumDate, supplierTimeZone.Settings.Offset).Value;
            ShiftedMinimumDate.Set(context, shiftedDate);
        }

        private DateTime? ShiftDateTime(DateTime? date, TimeSpan offSet)
        {
            if (!date.HasValue) return null;
            return date.Value.Subtract(offSet);
        }

    }
}
