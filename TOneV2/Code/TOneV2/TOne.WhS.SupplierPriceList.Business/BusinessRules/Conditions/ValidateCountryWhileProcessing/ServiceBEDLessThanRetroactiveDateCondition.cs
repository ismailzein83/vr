using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ServiceBEDLessThanRetroactiveDateCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is AllImportedCountries;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CountryManager countryManager = new CountryManager();
            bool isValid = true;
            var allImportedCountries = context.Target as AllImportedCountries;
            var contextMessages = new List<string>();
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var invalidZonesByCountryName = new List<string>();
            string retroactiveDateString = importSPLContext.RetroactiveDate.ToString(importSPLContext.DateFormat);

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                var invalidZoneNames = new HashSet<string>();
            

                foreach (ImportedZone zoneData in importedCountry.ImportedZones)
                {
                    if (zoneData.ImportedZoneServiceGroup != null && zoneData.ImportedZoneServiceGroup.BED < importSPLContext.RetroactiveDate && zoneData.ImportedZoneServiceGroup.ChangeType == ZoneServiceChangeType.New)
                    {
                        invalidZoneNames.Add(zoneData.ZoneName);
                    }
                }
                if (invalidZoneNames.Count > 0)
                {
                    string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                    invalidZonesByCountryName.Add(string.Format("'{0} : {1}'", countryName, string.Join(",", invalidZoneNames)));
                }
            }

            if (invalidZonesByCountryName.Count > 0)
            {
                context.Message = string.Format("Adding services in dates less than retroactive date '{0}' for the following zone(s): '{1}'", retroactiveDateString, string.Join(" ; ", invalidZonesByCountryName));
                isValid = false;
            }

            return isValid;
        }
        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
