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

        public void TryAddValue(int ownertype, int ownerId, ExistingRate value)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            List<ExistingRate> matchedExistingRates;
            if (!this.TryGetValue(ownertype, ownerId, out matchedExistingRates))
            {
                matchedExistingRates = new List<ExistingRate>();
                this._existingRatesByOwner.Add(owner, matchedExistingRates);
            }
            matchedExistingRates.Add(value);
        }

        public bool TryGetValue(int ownertype, int ownerId, out List<ExistingRate> value)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            return _existingRatesByOwner.TryGetValue(owner, out value);
        }

        public Dictionary<string, List<ExistingRate>>.Enumerator GetEnumerator()
        {
            return this._existingRatesByOwner.GetEnumerator();
        }

        public Owner GetOwner(string key)
        {
            if (key == null)
                throw new NullReferenceException("key");

            string[] owner = key.Split(',');
            return new Owner()
            {
                OwnerId = int.Parse(owner[1]),
                OwnerType = (SalePriceListOwnerType)int.Parse(owner[0])
            };
        }   
    }

    public class ExistingZonesRoutingProductsByOwner
    {
        private Dictionary<string, List<ExistingZoneRoutingProducts>> _existingZonesRoutingProductsByOwner;

        public ExistingZonesRoutingProductsByOwner()
        {
            _existingZonesRoutingProductsByOwner = new Dictionary<string, List<ExistingZoneRoutingProducts>>();
        }

        public void TryAddValue(int ownertype, int ownerId, ExistingZoneRoutingProducts value)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            List<ExistingZoneRoutingProducts> matchedExistingRoutingProducts;
            if (!this.TryGetValue(ownertype, ownerId, out matchedExistingRoutingProducts))
            {
                matchedExistingRoutingProducts = new List<ExistingZoneRoutingProducts>();
                this._existingZonesRoutingProductsByOwner.Add(owner, matchedExistingRoutingProducts);
            }
            matchedExistingRoutingProducts.Add(value);
        }

        public bool TryGetValue(int ownertype, int ownerId, out List<ExistingZoneRoutingProducts> value)
        {
            string owner = string.Join<int>(",", new List<int>() { ownertype, ownerId });
            return _existingZonesRoutingProductsByOwner.TryGetValue(owner, out value);
        }

        public Dictionary<string, List<ExistingZoneRoutingProducts>>.Enumerator GetEnumerator()
        {
            return this._existingZonesRoutingProductsByOwner.GetEnumerator();
        }

        public Owner GetOwner(string key)
        {
            if (key == null)
                throw new NullReferenceException("key");

            string[] owner = key.Split(',');
            return new Owner()
            {
                OwnerId = int.Parse(owner[1]),
                OwnerType = (SalePriceListOwnerType)int.Parse(owner[0])
            };
        }
    }
}
