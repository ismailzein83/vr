﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRuntimeNodeConfigurationDataManager : IDataManager
    {
        List<RuntimeNodeConfiguration> GetAllNodeConfigurations();
        bool Insert(RuntimeNodeConfiguration runtimeNodeConfiguration);
        bool Update(RuntimeNodeConfiguration runtimeNodeConfiguration);
        bool AreRuntimeNodeConfigurationUpdated(ref object lastReceivedDataInfo);
    }
}
