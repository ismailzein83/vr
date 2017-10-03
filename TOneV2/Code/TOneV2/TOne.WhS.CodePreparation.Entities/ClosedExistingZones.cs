using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class ClosedExistingZones
    {
        private Dictionary<string, List<ExistingZone>> _closedExistingZones;

        public ClosedExistingZones()
        {
            _closedExistingZones = new Dictionary<string, List<ExistingZone>>();
        }

        public List<ExistingZone> TryAddValue(string zoneName, List<ExistingZone> existingZoneToAdd)
        {
            List<ExistingZone> existingZone;

            if (this._closedExistingZones.TryGetValue(zoneName, out existingZone))
                throw new DataIntegrityValidationException(string.Format("Could not have two zones with the same name : {0}", zoneName));


            lock (_closedExistingZones)
            {
                if (this._closedExistingZones.TryGetValue(zoneName, out existingZone))
                {
                    throw new DataIntegrityValidationException(string.Format("Could not have two zones with the same name : {0}", zoneName));
                }
                else
                {
                    this._closedExistingZones.Add(zoneName, existingZoneToAdd);
                    return existingZone;
                }
            }
        }
        public bool TryGetValue(string zoneName, out List<ExistingZone>  existingZone)
        {
            return this._closedExistingZones.TryGetValue(zoneName, out existingZone);
       }
        public Dictionary<string, List<ExistingZone>> GetClosedExistingZones()
        {
            return this._closedExistingZones;
        }

    }
}
