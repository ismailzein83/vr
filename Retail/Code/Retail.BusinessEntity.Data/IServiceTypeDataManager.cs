﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IServiceTypeDataManager:IDataManager
    {
        IEnumerable<ServiceType> GetServiceTypes();

        bool Update(ServiceType serviceType);

        bool AreServiceTypesUpdated(ref object updateHandle);
    }
}
