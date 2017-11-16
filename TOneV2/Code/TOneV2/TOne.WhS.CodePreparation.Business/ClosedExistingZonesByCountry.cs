using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class ClosedExistingZonesByCountry
    {
        private Dictionary<int, Dictionary<string, List<ExistingZone>>> _closedExistingZonesByCountry;

        public ClosedExistingZonesByCountry()
        {
            _closedExistingZonesByCountry = new Dictionary<int, Dictionary<string, List<ExistingZone>>>();
        }

        public Dictionary<string, List<ExistingZone>> TryAddValue(int countryId, Dictionary<string, List<ExistingZone>> countryClosedExistingZones)
        {
            Dictionary<string, List<ExistingZone>> closedExistingZones;

            if (this._closedExistingZonesByCountry.TryGetValue(countryId, out closedExistingZones))
                return closedExistingZones;

            lock (_closedExistingZonesByCountry)
            {
                if (this._closedExistingZonesByCountry.TryGetValue(countryId, out closedExistingZones))
                    return closedExistingZones;
                else
                {
                    this._closedExistingZonesByCountry.Add(countryId, countryClosedExistingZones);
                    return countryClosedExistingZones;
                }
            }
        }
        public bool TryGetValue(int countryId, out Dictionary<string, List<ExistingZone>> countryClosedExistingZones)
        {
            return this._closedExistingZonesByCountry.TryGetValue(countryId, out countryClosedExistingZones);
        }
        public Dictionary<int, Dictionary<string, List<ExistingZone>>> GetClosedExistingZones()
        {
            return this._closedExistingZonesByCountry;
        }

    }
}
