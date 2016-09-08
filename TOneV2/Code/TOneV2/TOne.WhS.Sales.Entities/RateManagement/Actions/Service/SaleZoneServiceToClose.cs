using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class SaleZoneServiceToClose
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public DateTime CloseEffectiveDate { get; set; }
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
