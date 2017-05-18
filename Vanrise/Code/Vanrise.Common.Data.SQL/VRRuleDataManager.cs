using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRRuleDataManager : BaseSQLDataManager, IVRRuleDataManager
    {
        public IEnumerable<VRRule> GetVRRules(int vrRuleDefinitionId)
        {
            throw new NotImplementedException();
        }

        public bool AddVRRule(VRRule vrRule, out int vrRuleId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRule(VRRule vrRule)
        {
            throw new NotImplementedException();
        }

        public bool AreRulesUpdated(int vrRuleDefinitionId, ref object updateHandle)
        {
            throw new NotImplementedException();
        }
    }
}
