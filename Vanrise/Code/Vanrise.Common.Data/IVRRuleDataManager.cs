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
        IEnumerable<VRRule> GetVRRules(Guid vrRuleDefinitionId);

        bool Insert(VRRule vrRule, out long vrRuleId);

        bool Update(VRRule vrRule);

        bool AreRulesUpdated(Guid vrRuleDefinitionId, ref object updateHandle);
    }
}
