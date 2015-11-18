using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class Zone
    {
        public long SupplierZoneId { get; set; }

        public int SupplierId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }

        
        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
        public List<Code> Codes { get; set; }

        public List<Rate> Rates { get; set; }
        public Status Status { get; set; }

        public PriceListRateItem NewRate { get; set; }
        public List<PriceListCodeItem> NewCodes { get; set; }
        
    }
}
