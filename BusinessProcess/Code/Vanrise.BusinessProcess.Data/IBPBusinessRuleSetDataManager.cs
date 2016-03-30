﻿using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPBusinessRuleSetDataManager : IDataManager
    {
        List<BPBusinessRuleSet> GetBPBusinessRuleSets();

        bool AreBPBusinessRuleSetsUpdated(ref object updateHandle);

        bool AddBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj, out int bpBusinessRuleSetId);

        bool UpdateBusinessRuleSet(BPBusinessRuleSet businessRuleSetObj);
    }
}