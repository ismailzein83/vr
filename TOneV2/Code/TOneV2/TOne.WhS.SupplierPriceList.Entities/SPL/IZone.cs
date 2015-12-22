using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings
    {
        long ZoneId { get; }

        string Name { get; }

        List<NewCode> NewCodes { get; }

        List<NewRate> NewRates { get; }

        int CountryId { get; }
    }

    public class ZonesByName : Dictionary<string, List<IZone>>
    {

    }
}
