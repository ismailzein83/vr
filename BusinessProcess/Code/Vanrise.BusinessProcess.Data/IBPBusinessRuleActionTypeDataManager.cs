using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPBusinessRuleActionTypeDataManager : IDataManager
    {
        List<BPBusinessRuleActionType> GetBPBusinessRuleActionTypes();

        bool AreBPBusinessRuleActionTypesUpdated(ref object updateHandle);
    }
}