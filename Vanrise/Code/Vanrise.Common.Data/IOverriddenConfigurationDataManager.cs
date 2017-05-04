﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IOverriddenConfigurationDataManager : IDataManager
    {
        List<OverriddenConfiguration> GetOverriddenConfigurations();
        bool Update(OverriddenConfiguration overriddenConfiguration);
        bool Insert(OverriddenConfiguration overriddenConfiguration);
        bool AreOverriddenConfigurationsUpdated(ref object updateHandle);
    }
}
