using System.Collections.Generic;

namespace TABS.Addons.PriceListExport
{
    public interface IZoneSheetGenerator
    {
        byte[] GetZoneSheet(List<TABS.Zone> zones, SecurityEssentials.User CusrrentUser);
    }
}
