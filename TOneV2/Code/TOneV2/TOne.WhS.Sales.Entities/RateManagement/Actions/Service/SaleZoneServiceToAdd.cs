using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneServiceToAdd : Vanrise.Entities.IDateEffectiveSettings
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<ZoneService> Services { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }

        private List<NewSaleZoneService> _newSaleZoneServices = new List<NewSaleZoneService>();
        public List<NewSaleZoneService> NewSaleZoneServices
        {
            get
            {
                return _newSaleZoneServices;
            }
        }

        private List<ExistingSaleZoneService> _changedExistingSaleZoneServices = new List<ExistingSaleZoneService>();
        public List<ExistingSaleZoneService> ChangedExistingSaleZoneServices
        {
            get
            {
                return _changedExistingSaleZoneServices;
            }
        }
    }
}
