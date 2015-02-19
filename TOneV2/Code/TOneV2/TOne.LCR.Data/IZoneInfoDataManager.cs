using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.LCR.Data
{
    public interface IZoneInfoDataManager : IDataManager, IRoutingDataManager
    {
        Object PrepareZoneInfosForDBApply(List<ZoneInfo> zoneInfos);
        void ApplyZoneInfosToDB(Object preparedZoneInfos);
    }
}
