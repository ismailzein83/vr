using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Data
{
    public interface IZoneDataManager : IDataManager
    {
        void InsertZoneFromSource(Zone zone);
        void UpdateZoneFromSource(Zone zone);
        List<Zone> GetZones();
        bool AreZonesUpdated(ref object updateHandle);
    }
}
