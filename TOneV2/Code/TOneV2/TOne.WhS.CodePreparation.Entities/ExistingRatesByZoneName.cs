using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
   public class ExistingRatesByZoneName
    {
       private Dictionary<string, List<ExistingRate>> _existingRatesByZoneName;

       public ExistingRatesByZoneName()
        {
            this._existingRatesByZoneName = new Dictionary<string, List<ExistingRate>>();

        }

       public void Add(string key, List<ExistingRate> value)
        {
            _existingRatesByZoneName.Add(key.ToLower(), value);
        }

       public bool TryGetValue(string key, out List<ExistingRate> value)
        {
            return _existingRatesByZoneName.TryGetValue(key.ToLower(), out value);
        }
    }
}
