using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public interface IZone : Vanrise.Entities.IDateEffectiveSettings
    {
        long ZoneId { get; }

        string Name { get; }

        List<AddedCode> AddedCodes { get; }
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

        public IEnumerable<AddedZone> GetNewZones()
        {
            return this._ZonesByName.SelectMany(itm => itm.Value.Where(izone => izone is AddedZone)).Select(itm => itm as AddedZone);
        }

    }
}
