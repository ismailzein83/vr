using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
namespace TOne.WhS.CDRProcessing.Business
{
    public class NormalizationRuleManager : Vanrise.Rules.RuleManager<NormalizationRule, NormalizationRuleDetail>
    {
        public IDataRetrievalResult<NormalizationRuleDetail> GetFilteredNormalizationRules(Vanrise.Entities.DataRetrievalInput<NormalizationRuleQuery> input)
        {
            var normalizationRules = base.GetAllRules();

            Func<NormalizationRule, bool> filterExpression = (item) =>
                (
                    input.Query.PhoneNumberPrefix == null ||
                    (item.Criteria.PhoneNumberPrefix != null && item.Criteria.PhoneNumberPrefix.Contains(input.Query.PhoneNumberPrefix))
                )
                &&
                (
                    input.Query.PhoneNumberLength == null ||
                    (item.Criteria.PhoneNumberLength != null && item.Criteria.PhoneNumberLength == input.Query.PhoneNumberLength)
                )
               &&
                (
                    input.Query.Description == null ||
                    (item.Description != null && item.Description.Contains(input.Query.Description))
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, normalizationRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override NormalizationRuleDetail MapToDetails(NormalizationRule rule)
        {
            return new NormalizationRuleDetail
            {
                Entity = rule
            };
        }
    }
}
