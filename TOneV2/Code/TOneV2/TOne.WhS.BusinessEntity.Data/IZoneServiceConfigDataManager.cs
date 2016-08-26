using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IZoneServiceConfigDataManager: IDataManager
    {
        List<ZoneServiceConfig> GetZoneServiceConfigs();

        bool Update(ZoneServiceConfig zoneServiceFlag);

        bool Insert(Entities.ZoneServiceConfig zoneServiceFlag, out int insertedId);

        bool AreZoneServiceConfigsUpdated(ref object updateHandle);


    }
}
