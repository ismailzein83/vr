using System;
using System.Collections.Generic;

namespace TABS.Addons.PriceListExport
{
    public interface ICodeSheetGenerator
    {
        byte[] GetCodeSheetWorkbook(IEnumerable<ZoneCodeNotes> data, CarrierAccount Customer, DateTime EffectiveDate, SecurityEssentials.User CusrrentUser);
    }
}
