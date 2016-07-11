using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingZoneInfo
    {
        public int CountryId { get; set; }
    }


    public class ExistingZoneInfoByZoneName
    {
        private Dictionary<string, ExistingZoneInfo> _existingZonesInfoByName;

        public ExistingZoneInfoByZoneName()
        {
            _existingZonesInfoByName = new Dictionary<string, ExistingZoneInfo>();
        }
        public void Add(string key, ExistingZoneInfo values)
        {
            _existingZonesInfoByName.Add(key.ToLower(), values);
        }

        public bool TryGetValue(string key, out ExistingZoneInfo value)
        {
            return _existingZonesInfoByName.TryGetValue(key.ToLower(), out value);
        }

    }


}
