﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICommissionDataManager : IDataManager
    {
        List<Commission> GetCommission(string customerId, int zoneId, DateTime when);
    }
}
