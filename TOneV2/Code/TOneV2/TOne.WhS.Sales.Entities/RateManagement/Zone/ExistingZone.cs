using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingZone : Vanrise.Entities.IDateEffectiveSettings
    {
        public BusinessEntity.Entities.SaleZone ZoneEntity { get; set; }
        public long ZoneId
        {
            get { return ZoneEntity.SaleZoneId; }
        }
        public string Name
        {
            get { return ZoneEntity.Name; }
        }
        public int CountryId
        {
            get { return ZoneEntity.CountryId; }
        }
        public DateTime BED
        {
            get { return ZoneEntity.BED; }
        }
        public DateTime? EED
        {
            get { return ZoneEntity.EED; }
        }

        List<NewRate> _newRates = new List<NewRate>();
        public List<NewRate> NewRates
        {
            get {
                return _newRates;
            }
        }

        List<ExistingRate> _existingRates = new List<ExistingRate>();
        public List<ExistingRate> ExistingRates
        {
            get
            {
                return _existingRates;
            }
        }

		private List<NewSaleZoneRoutingProduct> _newZoneRoutingProducts = new List<NewSaleZoneRoutingProduct>();
		public List<NewSaleZoneRoutingProduct> NewZoneRoutingProducts
		{
			get
			{
				return _newZoneRoutingProducts;
			}
		}

		private List<ExistingSaleZoneRoutingProduct> _existingZoneRoutingProducts = new List<ExistingSaleZoneRoutingProduct>();
		public List<ExistingSaleZoneRoutingProduct> ExistingZoneRoutingProducts
		{
			get
			{
				return _existingZoneRoutingProducts;
			}
		}
    }

    public class ExistingZonesByName
    {
        private Dictionary<string, List<ExistingZone>> _existingZonesByName;
        public ExistingZonesByName()
        {
            _existingZonesByName = new Dictionary<string, List<ExistingZone>>();
        }
        public void Add(string key, List<ExistingZone> values)
        {
            _existingZonesByName.Add(key.ToLower(), values);
        }
        public bool TryGetValue(string key, out List<ExistingZone> value)
        {
            return _existingZonesByName.TryGetValue(key.ToLower(), out value);
        }
    }
}
