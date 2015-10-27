using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class SupplierIdentificationRuleManager : Vanrise.Rules.RuleManager<SupplierIdentificationRule, SupplierIdentificationRuleDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<SupplierIdentificationRuleDetail> GetFilteredSupplierIdentificationRules(Vanrise.Entities.DataRetrievalInput<SupplierIdentificationRuleQuery> input)
        {
            Func<SupplierIdentificationRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SupplierIdentificationRuleDetail MapToDetails(SupplierIdentificationRule rule)
        {
            return new SupplierIdentificationRuleDetail{
                Entity=rule
            };
        }
    }
}
