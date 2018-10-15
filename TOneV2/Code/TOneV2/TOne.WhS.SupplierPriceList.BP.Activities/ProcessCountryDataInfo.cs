using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class ProcessCountryDataInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CountryId { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<NewCode>> NewCodes { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedCode>> NotImportedCodes { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewZone>> NewZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }
      
        [RequiredArgument]
        public InArgument<IEnumerable<NotImportedZone>> NotImportedZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewZoneService>> NewZoneServices { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedZone>> ChangedZones { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedZoneService>> ChangedZoneServices { get; set; }
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }
        [RequiredArgument]
        public InArgument<Dictionary<int, ProcessedCountryDataInfo>> CountryDataByCountryId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int countryId = this.CountryId.Get(context);
            IEnumerable<NewCode> newCodes = this.NewCodes.Get(context);
            IEnumerable<NewZone> newZones = this.NewZones.Get(context);
            IEnumerable<NewRate> newRates = this.NewRates.Get(context);
            IEnumerable<NewZoneService> newZoneServices = this.NewZoneServices.Get(context);
            IEnumerable<ChangedCode> changedCodes = this.ChangedCodes.Get(context);
            IEnumerable<ChangedZone> changedZones = this.ChangedZones.Get(context);
            IEnumerable<ChangedZoneService> changedZoneServices = this.ChangedZoneServices.Get(context);
            IEnumerable<ChangedRate> changedRates = this.ChangedRates.Get(context);
            IEnumerable<NotImportedCode> notImportedCodes = this.NotImportedCodes.Get(context);
            Dictionary<int, ProcessedCountryDataInfo> countryDataByCountryId = this.CountryDataByCountryId.Get(context);
            IEnumerable<NotImportedZone> notImportedZones = this.NotImportedZones.Get(context);

            var countryData = countryDataByCountryId.GetRecord(countryId);
            if (countryData != null)
            {
                countryData.NewCodes = newCodes != null ? newCodes : new List<NewCode>();
                countryData.NewZones = newZones != null ? newZones : new List<NewZone>();
                countryData.NewRates = newRates != null ? newRates : new List<NewRate>();
                countryData.NewZonesServices = newZoneServices != null ? newZoneServices : new List<NewZoneService>();
                countryData.ChangedCodes = changedCodes;
                countryData.ChangedRates = changedRates;
                countryData.ChangedZones = changedZones;
                countryData.ChangedZoneServices = changedZoneServices;
                countryData.NotImportedCodes = notImportedCodes.ToList();
                countryData.NotImportedZones = notImportedZones.ToList();
            }
        

            this.CountryDataByCountryId.Set(context, countryDataByCountryId);
        }
    }
}
