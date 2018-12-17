﻿using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IBPDefinitionDataManager : IDataManager
    {
        List<BPDefinition> GetBPDefinitions();

        bool InsertBPDefinition(BPDefinition bpDefinition);

        bool UpdateBPDefinition(BPDefinition bpDefinition);
        bool AreBPDefinitionsUpdated(ref object lastReceivedDataInfo);
    }
}