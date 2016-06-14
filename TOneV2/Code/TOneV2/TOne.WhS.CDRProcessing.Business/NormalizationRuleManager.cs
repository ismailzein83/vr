﻿using System;
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
                    input.Query.PhoneNumberTypes==null || (item.Criteria.PhoneNumberTypes.Any(x=> input.Query.PhoneNumberTypes.Contains(x)))
                )
                &&
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
                )
                 &&
                (
                    input.Query.EffectiveDate == null ||
                    (
                        item.BeginEffectiveTime <= input.Query.EffectiveDate &&
                        (item.EndEffectiveTime == null || item.EndEffectiveTime >= input.Query.EffectiveDate)
                    )
                );

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, normalizationRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        public override NormalizationRuleDetail MapToDetails(NormalizationRule rule)
        {
            return new NormalizationRuleDetail
            {
                Entity = rule
            };
        }
    }
}
