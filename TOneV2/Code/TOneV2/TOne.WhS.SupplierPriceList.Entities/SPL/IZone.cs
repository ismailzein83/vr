using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using System.Linq;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings
    {

        long ZoneId { get; }

        string Name { get; }

        List<NewCode> NewCodes { get; }

        List<NewRate> NewRates { get; }

        List<NewZoneService> NewZoneServices { get; }

        int CountryId { get; }
    }

    public class ZonesByName
    {
        private Dictionary<string, List<IZone>> _ZonesByName;

        public ZonesByName()
        {
            _ZonesByName = new Dictionary<string, List<IZone>>();
        }
        public void Add(string key, List<IZone> values)
        {
            _ZonesByName.Add(key.ToLower(), values);
        }

        public bool TryGetValue(string key, out List<IZone> value)
        {
            value = new List<IZone>();
            return _ZonesByName.TryGetValue(key.ToLower(), out value);
        }

        public IEnumerable<NewZone> GetNewZones()
        {
            return this._ZonesByName.SelectMany(itm => itm.Value.Where(izone => izone is NewZone)).Select(itm => itm as NewZone);
        }
    }
}
