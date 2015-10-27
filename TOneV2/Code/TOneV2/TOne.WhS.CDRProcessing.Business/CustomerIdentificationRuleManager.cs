using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class CustomerIdentificationRuleManager : Vanrise.Rules.RuleManager<CustomerIdentificationRule, CustomerIdentificationRuleDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<CustomerIdentificationRuleDetail> GetFilteredCustomerIdentificationRules(Vanrise.Entities.DataRetrievalInput<CustomerIdentificationRuleQuery> input)
        {
            Func<CustomerIdentificationRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override CustomerIdentificationRuleDetail MapToDetails(CustomerIdentificationRule rule)
        {
            return new CustomerIdentificationRuleDetail
            {
                Entity = rule
            };
        }
    }
}
