using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class RateToAdd
    {
        public string ZoneName { get; set; }

        public PriceListToAdd PriceListToAdd { get; set; }

        public decimal Rate { get; set; }

        private List<AddedRate> _addedRate = new List<AddedRate>();

        public List<AddedRate> AddedRates
        {
            get {
                return this._addedRate;
            }
        }
    }
}
