using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeGroupVerificationCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryNotImportedCodes;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            CountryNotImportedCodes allCodes = context.Target as CountryNotImportedCodes;

            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var supplierPriceListType = importSPLContext.SupplierPricelistType;

            bool codeGroupExistInNotImportedData = false;

            string mainBreakoutCode = null;
            string zoneName = null;

            IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(allCodes.CountryId);
            foreach (NotImportedCode notImportedCode in allCodes.NotImportedCodes)
            {
                if (codeGroupCodes.Any(x => x.Code == notImportedCode.Code))
                {
                    mainBreakoutCode = notImportedCode.Code;
                    zoneName = notImportedCode.ZoneName;
                    codeGroupExistInNotImportedData = true;
                    break;
                }
            }
            var pricelistCodeManager = new PriceListCodeManager();

            if (supplierPriceListType == SupplierPricelistType.RateChange || !codeGroupExistInNotImportedData || pricelistCodeManager.IsCodeGroupVerificationEnabled())
            {
                return true;
            }

            CountryManager countryManager = new CountryManager();
            string countryName = countryManager.GetCountryName(allCodes.CountryId);

            context.Message = string.Format("Country '{2}' has code group '{0}' ended in zone '{1}'", mainBreakoutCode, zoneName, countryName);
            return false;

        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}






