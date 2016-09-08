using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingSaleZoneService : Vanrise.Entities.IDateEffectiveSettings
    {
        public SaleEntityZoneService SaleEntityZoneServiceEntity { get; set; }
        public ChangedSaleZoneService ChangedSaleZoneService { get; set; }
        public DateTime BED
        {
            get { return SaleEntityZoneServiceEntity.BED; }
        }
        public DateTime? EED
        {
            get { return ChangedSaleZoneService != null ? ChangedSaleZoneService.EED : SaleEntityZoneServiceEntity.EED; }
        }
    }

    public class ExistingSaleZoneServicesByZoneName
    {
        private Dictionary<string, List<ExistingSaleZoneService>> _existingSaleZoneServicesByZoneName;

        public ExistingSaleZoneServicesByZoneName()
        {
            _existingSaleZoneServicesByZoneName = new Dictionary<string, List<ExistingSaleZoneService>>();
        }

        public void Add(string key, List<ExistingSaleZoneService> value)
        {
            _existingSaleZoneServicesByZoneName.Add(key.ToLower(), value);
        }

        public bool TryGetValue(string key, out List<ExistingSaleZoneService> value)
        {
            return _existingSaleZoneServicesByZoneName.TryGetValue(key.ToLower(), out value);
        }
    }
}
