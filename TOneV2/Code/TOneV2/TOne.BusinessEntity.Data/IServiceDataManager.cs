﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface IServiceDataManager : IDataManager
    {
        Dictionary<short, FlaggedService> GetServiceFlags();

        FlaggedService GetServiceFlag(short id);
        string GetServicesDisplayList(short serviceFlag);
    }
}
