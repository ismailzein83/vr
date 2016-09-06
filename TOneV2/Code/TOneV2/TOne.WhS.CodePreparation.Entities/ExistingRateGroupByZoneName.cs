using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class ExistingRateGroupByZoneName
    {
        private Dictionary<string, ExistingRateGroup> _existingRateGroupByZoneName;

        public ExistingRateGroupByZoneName()
        {
            this._existingRateGroupByZoneName = new Dictionary<string, ExistingRateGroup>();

        }

        public void Add(string key, ExistingRateGroup value)
        {
            _existingRateGroupByZoneName.Add(key.ToLower(), value);
        }

        public bool TryGetValue(string key, out ExistingRateGroup value)
        {
            return _existingRateGroupByZoneName.TryGetValue(key.ToLower(), out value);
        }
    }
}
