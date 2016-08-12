using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Vanrise.Notification.Business
{
    public class VRAlertRuleManager
    {
        public List<VRAlertRule> GetActiveRules(Guid ruleTypeId)
        {
            return GetCachedRulesByType().GetRecord(ruleTypeId);
        }

        private Dictionary<Guid, List<VRAlertRule>> GetCachedRulesByType()
        {
            throw new NotImplementedException();
        }

        private Dictionary<long, VRAlertRule> GetCachedRules()
        {
            throw new NotImplementedException();
        }
    }
}
