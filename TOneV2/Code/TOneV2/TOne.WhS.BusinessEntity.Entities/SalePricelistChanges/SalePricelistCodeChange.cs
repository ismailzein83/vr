using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricelistCodeChange
    {
        public string ZoneName { get; set; }

        public string RecentZoneName { get; set; }

        public int CountryId { get; set; }

        public string Code { get; set; }

        //TODO: put it in Sale Code on Business entity and remove it from Numbering Plan
        //public CodeChaneType ChangeType { get; set; }

        public int PricelistId { get; set; }
    }
}
