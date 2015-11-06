﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IZoneServiceConfig: IDataManager
    {
        List<ZoneServiceConfig> GetZoneServiceConfigs();

        bool Update(ZoneServiceConfig zoneServiceFlag);

        bool Insert(ZoneServiceConfig zoneServiceFlag);

        bool AreZoneServiceConfigUpdated(ref object updateHandle);


    }
}
