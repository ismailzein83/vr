using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
    public class ExistingRatesByOwner
    {
        private Dictionary<string, List<ExistingRate>> _existingRatesByOwner;

        public ExistingRatesByOwner()
        {
            _existingRatesByOwner = new Dictionary<string, List<ExistingRate>>();
        }

        public void Add(int ownertype, int ownerId, List<ExistingRate> values)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            _existingRatesByOwner.Add(owner, values);
        }

        public bool TryGetValue(int ownertype, int ownerId, out List<ExistingRate> value)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            return _existingRatesByOwner.TryGetValue(owner, out value);
        }

        public IEnumerable<Owner> GetOwners()
        {
            List<Owner> owners = new List<Owner>();
            foreach (string key in _existingRatesByOwner.Keys)
            {
                string[] owner = key.Split(',');
                owners.Add(new Owner()
                {
                    OwnerType = int.Parse(owner[0]),
                    OwnerId = int.Parse(owner[1])
                });
            }
            return owners;
        }
   
    }
}
