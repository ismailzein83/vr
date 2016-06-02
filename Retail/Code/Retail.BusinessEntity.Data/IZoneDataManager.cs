using Retail.BusinessEntity.Entities;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Data
{
    public interface IZoneDataManager : IDataManager
    {
        bool AreZonesUpdated(ref object updateHandle);
        List<Zone> GetZones();

    }
}
