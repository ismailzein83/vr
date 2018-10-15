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

namespace TOne.WhS.SupplierPriceList.Business
{
    public class NewZonesWithoutCodeGroupsCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as AllImportedCountries != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            bool isValid = true;
            CountryManager countryManager = new CountryManager();
            var contextMessages = new List<string>();
            var allImportedCountries = context.Target as AllImportedCountries;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var supplierPriceListType = importSPLContext.SupplierPricelistType;
            var zonesWithoutCodeGroup = new List<string>();

            foreach (var importedCountry in allImportedCountries.ImportedCountries)
            {
                List<string> newZones = new List<string>();

                if (supplierPriceListType != SupplierPricelistType.RateChange)
                {
                    bool codeGroupExist = false;
                    IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(importedCountry.CountryId);
                    var importedCodes = importedCountry.ImportedCodes;
                    if (importedCodes.Count() > 0)
                    {
                        foreach (var codeGroup in codeGroupCodes)
                        {
                            if (importedCodes.Any(x => x.Code == codeGroup.Code))
                            {
                                codeGroupExist = true;
                                break;
                            }
                        }
                        if (codeGroupExist == false)
                        {
                            foreach (var importedZone in importedCountry.ImportedZones)
                            {
                                if (importedZone.ChangeType == ZoneChangeType.New)
                                    newZones.Add(importedZone.ZoneName);
                            }
                        }
                    }

                }
                if (newZones.Count > 0)
                {
                    string countryName = countryManager.GetCountryName(importedCountry.CountryId);
                    zonesWithoutCodeGroup.Add(string.Format("'{0} : {1}'",countryName, string.Join(",", newZones)));
                }
            }
            if (zonesWithoutCodeGroup.Count > 0)
            {
                context.Message = string.Format("Following zone(s) are added without code group: '{0}'", string.Join(" ; ", zonesWithoutCodeGroup));
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
