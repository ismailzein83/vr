using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class RateToAdd
    {
        public RateToAdd()
        {
            AddedRates = new List<AddedRate>();
        }
        public string ZoneName { get; set; }

        public PriceListToAdd PriceListToAdd { get; set; }

        public decimal Rate { get; set; }

        public List<AddedRate> AddedRates { get; set; }
    }
}
