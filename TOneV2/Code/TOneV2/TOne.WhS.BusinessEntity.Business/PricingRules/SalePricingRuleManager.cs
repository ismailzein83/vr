using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business.PricingRules
{
    public class SalePricingRuleManager : BasePricingRuleManager<SalePricingRule>
    {
        protected override IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors()
        {
            throw new NotImplementedException();
        }
        public Vanrise.Entities.IDataRetrievalResult<SalePricingRule> GetFilteredSalePricingRules(Vanrise.Entities.DataRetrievalInput<object> input)
        {
            Func<SalePricingRule, bool> filterExpression = (prod) =>
                 //(input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 //&&
                 //(input.Query.CarrierProfilesIds == null || input.Query.CarrierProfilesIds.Contains(prod.CarrierProfileId))
                 // &&
                 //(input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(prod.CarrierAccountId))
                 //  &&
                 (input.Query == null || input.Query=="");

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, GetFilteredRules(filterExpression).ToBigResult(input, filterExpression));
        }
    }
}
