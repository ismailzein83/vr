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
            return (target as ImportedCountry != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCountry country = context.Target as ImportedCountry;
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var supplierPriceListType = importSPLContext.SupplierPricelistType;
            List<string> newZones = new List<string>();

            if (supplierPriceListType != SupplierPricelistType.RateChange)
            {
                bool codeGroupExist = false;
                IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(country.CountryId);
                var importedCodes = country.ImportedCodes;
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
                        foreach (var importedZone in country.ImportedZones)
                        {
                            if (importedZone.ChangeType == ZoneChangeType.New)
                                newZones.Add(importedZone.ZoneName);
                        }
                    }
                }

            }
            if (newZones.Count > 0)
            {
                CountryManager countryManager = new CountryManager();
                string countryName = countryManager.GetCountryName(country.CountryId);
                context.Message = string.Format("For country '{0}' zone(s) : '{1}' are added without code group", countryName, string.Join(",", newZones));
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
