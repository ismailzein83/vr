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
            return true;
            //CountryNotImportedCodes allCodes = context.Target as CountryNotImportedCodes;

            //IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            //var supplierPriceListType = importSPLContext.SupplierPricelistType;

            //bool codeGroupExistInNotImportedData = false;

            //IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(allCodes.CountryId);
            //foreach (NotImportedCode notImportedCode in allCodes.NotImportedCodes)
            //{
            //    if (codeGroupCodes.Any(x => x.Code == notImportedCode.Code))
            //    {
            //        var mainBreakout = notImportedCode.Code;
            //        codeGroupExistInNotImportedData = true;
            //    }
            //}
            //var pricelistCodeManager = new PriceListCodeManager();

            //if (supplierPriceListType == SupplierPricelistType.RateChange || !codeGroupExistInNotImportedData || pricelistCodeManager.IsCodeGroupVerificationEnabled())
            //{
            //    return true;
            //}

            //else
            //{
            //    CountryManager countryManager = new CountryManager();
            //    string countryName = countryManager.GetCountryName(allCodes.CountryId);


            //    context.Message = string.Format("Code Group (x) of country y from Zone z will be removed");
            //    return false;
            //}

            //if ((supplierPriceListType != SupplierPricelistType.RateChange) && codeGroupExistInNotImportedData && !pricelistCodeManager.IsCodeGroupVerificationEnabled())
            //{
            //    context.Message = string.Format("Code Group of country {} will be removed");
            //    return false;
            //}
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}






