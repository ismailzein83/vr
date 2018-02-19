using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class SameZoneWithDifferentRatesCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone zone = context.Target as ImportedDataByZone;
            var invalidZones = new HashSet<string>();
            var messages = new List<string>();
            var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

            var distinctImportedNormalRates = from importedRate in zone.ImportedNormalRates
                                              where !importedRate.RateTypeId.HasValue
                                              group importedRate by importedRate.Rate into newRates
                                              select newRates;
            if (distinctImportedNormalRates.Count() > 1)
                messages.Add(string.Join(",", distinctImportedNormalRates.Select(item => item.Key)));

            foreach (KeyValuePair<int, List<ImportedRate>> kvp in zone.ImportedOtherRates)
            {
                var distinctImportedOtherRates = from importedRate in kvp.Value
                                                 group importedRate by importedRate.Rate into newRates
                                                 select newRates;

                if (distinctImportedOtherRates.Count() > 1)
                {
                    string rateTypeName = rateTypeManager.GetRateTypeName(kvp.Key);
                    messages.Add(string.Format("{0} ({1})", rateTypeName, string.Join(", ", distinctImportedOtherRates.Select(item => item.Key))));
                }
            }

            if (messages.Count > 0)
            {
                context.Message = string.Format("Zone '{0}' has different rates: {1}.", zone.ZoneName, string.Join(", ", messages));
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
