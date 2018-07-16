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

          
            List<string> codesByZone=new List<string>();
            List<string> codes=new List<string>();

            IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(allCodes.CountryId);
            foreach (NotImportedCode notImportedCode in allCodes.NotImportedCodes)
            {
                foreach (var codeGroup in codeGroupCodes)
                {
                    if (codeGroup.Code == notImportedCode.Code && notImportedCode.HasChanged==true)
                    {
                        codes.Add(notImportedCode.Code);
                        if (codeGroupExistInNotImportedData == false)
                            codeGroupExistInNotImportedData = true;
                    }
                }
                var zoneCodes = string.Format("'{0}({1})'", notImportedCode.ZoneName, string.Join<string>(",", codes));
                codesByZone.Add(zoneCodes);
            }
            var pricelistCodeManager = new PriceListCodeManager();

            if (supplierPriceListType == SupplierPricelistType.RateChange || !codeGroupExistInNotImportedData || pricelistCodeManager.IsCodeGroupVerificationEnabled())
            {
                return true;
            }

            CountryManager countryManager = new CountryManager();
            string countryName = countryManager.GetCountryName(allCodes.CountryId);

            context.Message = string.Format("In country '{0}',the following main breakout zone(s):'{1}',are closed.", countryName, string.Join<string>(",", codesByZone));
            return false;

        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}






