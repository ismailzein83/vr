using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;

namespace TOne.WhS.CodePreparation.Entities
{
    public class AddedRate
    {
        public long RateId { get; set; }

        public PriceListToAdd PriceListToAdd { get; set; }

        public AddedZone AddedZone { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
