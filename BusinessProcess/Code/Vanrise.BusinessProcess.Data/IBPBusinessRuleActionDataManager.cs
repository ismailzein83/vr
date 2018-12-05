using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPBusinessRuleActionDataManager : IDataManager
    {
        List<BPBusinessRuleAction> GetBPBusinessRuleActions();
    }
}