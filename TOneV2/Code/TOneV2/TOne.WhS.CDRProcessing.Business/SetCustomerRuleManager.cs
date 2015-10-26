using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class SetCustomerRuleManager : Vanrise.Rules.RuleManager<SetCustomerRule, SetCustomerRuleDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<SetCustomerRuleDetail> GetFilteredSetCustomerRules(Vanrise.Entities.DataRetrievalInput<SetCustomerRuleQuery> input)
        {
            Func<SetCustomerRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SetCustomerRuleDetail MapToDetails(SetCustomerRule rule)
        {
            return new SetCustomerRuleDetail
            {
                Entity = rule
            };
        }
    }
}
