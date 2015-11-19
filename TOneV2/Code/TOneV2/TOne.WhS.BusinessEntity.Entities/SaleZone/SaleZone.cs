using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum Status{New=0,Deleted=1 , Replace = -1}
    public class SaleZone
    {
        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }
        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public List<SaleCode> Codes { get; set; }

        public Status Status { get; set; }
    }
}
