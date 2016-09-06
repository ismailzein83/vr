using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class NotImportedRate
    {
        public string ZoneName { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }
        
        public int OwnerId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public bool HasChanged { get; set; }

        public int? RateTypeId { get; set; }

        public decimal Rate { get; set; }

    }
}
