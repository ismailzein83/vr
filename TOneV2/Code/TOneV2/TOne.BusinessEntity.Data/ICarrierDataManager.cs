﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICarrierDataManager : IDataManager
    {
        List<CarrierAccountInfo> GetActiveSuppliersInfo();

        List<CarrierInfo> GetCarriers(CarrierType carrierType);

        string GetCarrierAccountName(string carrierAccountId);
    }
}
