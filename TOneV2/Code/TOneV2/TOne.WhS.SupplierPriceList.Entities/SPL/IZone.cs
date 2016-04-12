using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings, IRuleTarget
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
