using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRRuleDataManager : IDataManager
    {
        IEnumerable<VRRule> GetVRRules(int vrRuleDefinitionId);

        bool AddVRRule(VRRule vrRule, out int vrRuleId);

        bool UpdateRule(VRRule vrRule);

        bool AreRulesUpdated(int vrRuleDefinitionId, ref object updateHandle);
    }
}
