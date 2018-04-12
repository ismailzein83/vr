using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPriceListChange
    {
        public int CustomerId { get; set; }

        public int PriceListId { get; set; }


        private List<SalePricelistCodeChange> _codeChanges = new List<SalePricelistCodeChange>();
        public List<SalePricelistCodeChange> CodeChanges { get { return this._codeChanges; } }

        private List<SalePricelistRateChange> _rateChanges = new List<SalePricelistRateChange>();
        public List<SalePricelistRateChange> RateChanges { get { return this._rateChanges; } }

        private List<SalePricelistRPChange> _routingProductChanges = new List<SalePricelistRPChange>();
        public List<SalePricelistRPChange> RoutingProductChanges { get { return this._routingProductChanges; } }
    }
    public class StructuredCustomerPricelistChange
    {
        public int CustomerId { get; set; }
        public List<CountryGroup> CountryGroups { get; set; }
    }

    public class CountryGroup
    {
        public int CountryId { get; set; }

        public List<SalePricelistCodeChange> _codeChanges = new List<SalePricelistCodeChange>();

        public List<SalePricelistCodeChange> CodeChanges
        {
            get { return _codeChanges; }
            set { _codeChanges = value; }
        }

        public List<SalePricelistRateChange> _rateChanges = new List<SalePricelistRateChange>();

        public List<SalePricelistRateChange> RateChanges
        {
            get { return _rateChanges; }
            set { _rateChanges = value; }
        }

        public List<SalePricelistRPChange> _rPChanges = new List<SalePricelistRPChange>();

        public List<SalePricelistRPChange> RPChanges
        {
            get { return _rPChanges; }
            set { _rPChanges = value; }
        }

    }
    public class NewCustomerPriceListChange
    {
        public int CustomerId { get; set; }
        public IEnumerable<PriceListChange> PriceLists { get; set; }
    }
    public class PriceListChange
    {
        public NewPriceList PriceList { get; set; }
        public int CurrencyId { get; set; }

        private List<CountryChange> _countryChanges = new List<CountryChange>();
        public List<CountryChange> CountryChanges
        {
            get { return _countryChanges; }
        }
    }

    public class NewPriceList
    {
        public long PriceListId { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public SalePriceListType? PriceListType { get; set; }
        public int OwnerId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public long ProcessInstanceId { get; set; }
        public long FileId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
    }

    public class CountryChange
    {
        public int CountryId { get; set; }

        private List<SalePricelistZoneChange> _zoneChanges = new List<SalePricelistZoneChange>();
        public List<SalePricelistZoneChange> ZoneChanges { get { return _zoneChanges; } }
    }

    public class SalePricelistZoneChange
    {
        public string ZoneName { get; set; }
        public long ZoneId { get; set; }

        private List<SalePricelistCodeChange> _codeChanges = new List<SalePricelistCodeChange>();
        public List<SalePricelistCodeChange> CodeChanges { get { return _codeChanges; } }
        private List<SalePricelistRateChange> _otherRateChanges = new List<SalePricelistRateChange>();
        public List<SalePricelistRateChange> OtherRateChanges { get { return _otherRateChanges; } }
        public SalePricelistRateChange RateChange { get; set; }
        public SalePricelistRPChange RPChange { get; set; }
    }
    public class SalePricelistZoneChangeByZoneName
    {
        private Dictionary<string, SalePricelistZoneChange> _salePricelistZoneChangeByZoneName;
        public SalePricelistZoneChangeByZoneName()
        {
            _salePricelistZoneChangeByZoneName = new Dictionary<string, SalePricelistZoneChange>();
        }
        public SalePricelistZoneChange TryAddValue(SalePricelistZoneChange zoneChange)
        {
            string zoneName = zoneChange.ZoneName.ToLower();

            foreach (var salePricelistZoneChange in _salePricelistZoneChangeByZoneName.Values)
            {
                if (salePricelistZoneChange.ZoneName.ToLower().Equals(zoneName))
                    return salePricelistZoneChange;
            }

            _salePricelistZoneChangeByZoneName.Add(zoneChange.ZoneName, zoneChange);
            return zoneChange;
        }
        public IEnumerable<SalePricelistZoneChange> GetZoneChanges()
        {
            return _salePricelistZoneChangeByZoneName.Values;
        }
    }
}
