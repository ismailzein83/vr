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
            return target is AllCountriesNotImportedCodes;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IImportSPLContext importSPLContext = context.GetExtension<IImportSPLContext>();
            var supplierPriceListType = importSPLContext.SupplierPricelistType;
            bool isValid = true;
            CountryManager countryManager = new CountryManager();
            var allCountriesNotImportedCodes = context.Target as AllCountriesNotImportedCodes;
            var contextMessages = new List<string>();
            var closedBreakoutsByCountryName = new List<string>();

            foreach (var countryNotImportedCodes in allCountriesNotImportedCodes.CountriesNotImportedCodes)
            {
                
                bool codeGroupExistInNotImportedData = false;


                List<string> codesByZone = new List<string>();
                List<string> codes = new List<string>();

                IEnumerable<CodeGroup> codeGroupCodes = new CodeGroupManager().GetCountryCodeGroups(countryNotImportedCodes.CountryId);
                if (countryNotImportedCodes.NotImportedCodes != null)
                {
                    var notImported = countryNotImportedCodes.NotImportedCodes.Where(c => c.HasChanged);
                    foreach (NotImportedCode notImportedCode in notImported)
                    {
                        foreach (var codeGroup in codeGroupCodes)
                        {
                            if (codeGroup.Code == notImportedCode.Code)
                            {
                                codes.Add(notImportedCode.Code);
                                if (codeGroupExistInNotImportedData == false)
                                    codeGroupExistInNotImportedData = true;
                            }
                        }
                        var zoneCodes = string.Format("'{0}({1})'", notImportedCode.ZoneName, string.Join<string>(",", codes));
                        codesByZone.Add(zoneCodes);
                    }
                }
                var pricelistCodeManager = new PriceListCodeManager();

                if (supplierPriceListType == SupplierPricelistType.RateChange || !codeGroupExistInNotImportedData || pricelistCodeManager.IsCodeGroupVerificationEnabled())
                    continue;

                if (codesByZone.Count > 0)
                {
                    string countryName = countryManager.GetCountryName(countryNotImportedCodes.CountryId);
                    closedBreakoutsByCountryName.Add(string.Format("'{0} : {1}'", countryName, string.Join(",", codesByZone)));
                }
            }


            if (closedBreakoutsByCountryName.Count > 0)
            {
                context.Message = string.Format("For the following country(ies), the main breakout zone(s) are closed : '{0}'", string.Join(" ; ", closedBreakoutsByCountryName));
                isValid = false;
            }
            return isValid;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}






