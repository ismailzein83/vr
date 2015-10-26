using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class SetSupplierRuleManager : Vanrise.Rules.RuleManager<SetSupplierRule, SetSupplierRuleDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<SetSupplierRuleDetail> GetFilteredSetSupplierRules(Vanrise.Entities.DataRetrievalInput<SetSupplierRuleQuery> input)
        {
            Func<SetSupplierRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SetSupplierRuleDetail MapToDetails(SetSupplierRule rule)
        {
            return new SetSupplierRuleDetail{
                Entity=rule
            };
        }
    }
}
