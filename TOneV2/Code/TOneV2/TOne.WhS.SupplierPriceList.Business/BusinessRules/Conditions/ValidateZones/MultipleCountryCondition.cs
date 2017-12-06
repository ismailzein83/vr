using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MultipleCountryCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            //ImportedDataByZone zone = context.Target as ImportedDataByZone;
            //var countries = new HashSet<string>();
            //CountryManager countryManager = new CountryManager ();
            //foreach (var importedCode in zone.ImportedCodes)
            //{
            //    if (!string.IsNullOrEmpty(importedCode.Code) && importedCode.CodeGroup != null)
            //        countries.Add(countryManager.GetCountryName(importedCode.CodeGroup.CountryId));
            //}

            //if (countries.Count>1)
            //{
            //    context.Message = string.Format("Can not add zone '{0}' with codes belong to different countries: {1}.", zone.ZoneName, string.Join(", ", countries));
            //    return false;
            //}

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
