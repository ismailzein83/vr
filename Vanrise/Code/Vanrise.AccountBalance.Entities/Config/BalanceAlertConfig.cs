using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAlertConfig
    {
        public int RuleDefinitionId { get; set; }

        public Dictionary<string, AccountRuleCriteriaSetting> CriteriaSettingsByCriteriaName { get; set; }
    }

    public class AccountRuleCriteriaSetting
    {
        public GenericData.Entities.BEPropertyEvaluator Evaluator { get; set; }
    }
}
