using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ProcessCountryCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<ImportedCode>> ImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCode>> ExistingCodes { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<long, ExistingZone>> ExistingZonesByZoneId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> DeletedCodesDate { get; set; }

        [RequiredArgument]
        public OutArgument<ZonesByName> NewAndExistingZones { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Dictionary<long, ExistingZone> existingZonesByZoneId = this.ExistingZonesByZoneId.Get(context);
            IEnumerable<ExistingZone> existingZones = null;

            if (existingZonesByZoneId != null)
                existingZones = existingZonesByZoneId.Select(item => item.Value);

            ProcessCountryCodesContext processCountryCodesContext = new ProcessCountryCodesContext()
            {
                ImportedCodes = this.ImportedCodes.Get(context),
                ExistingCodes = this.ExistingCodes.Get(context),
                ExistingZones = existingZones,
                DeletedCodesDate = this.DeletedCodesDate.Get(context)
            };

            PriceListCodeManager plCodeManager = new PriceListCodeManager();
            plCodeManager.ProcessCountryCodes(processCountryCodesContext);

            NewAndExistingZones.Set(context, processCountryCodesContext.NewAndExistingZones);
        }

        private class ProcessCountryCodesContext : IProcessCountryCodesContext
        {
            public IEnumerable<ImportedCode> ImportedCodes { get; set; }

            public IEnumerable<ExistingCode> ExistingCodes { get; set; }

            public IEnumerable<ExistingZone> ExistingZones { get; set; }

            public DateTime DeletedCodesDate { get; set; }

            public ZonesByName NewAndExistingZones { get; set; }
        }
    }
}
