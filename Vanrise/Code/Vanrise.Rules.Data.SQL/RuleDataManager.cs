using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Rules.Data.SQL
{
    public class RuleDataManager : BaseSQLDataManager, IRuleDataManager
    {
        public RuleDataManager()
            : base(GetConnectionStringName("RulesDBConnStringKey", "RulesDBConnString"))
        {

        }

        public bool AddRule(Entities.Rule rule, out int ruleId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRule(Entities.Rule ruleEntity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRule(int ruleId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Entities.Rule> GetRulesByType(int ruleTypeId)
        {
            throw new NotImplementedException();
        }

        public bool AreRulesUpdated(int ruleTypeId, ref object updateHandle)
        {
            return base.IsDataUpdated("rules.Rule", "TypeID", ruleTypeId, ref updateHandle);
        }


        public int GetRuleTypeId(string ruleType)
        {
            return (int)ExecuteScalarSP("rules.sp_RuleType_InsertIfNotExistsAndGetID", ruleType);
        }
    }
}
