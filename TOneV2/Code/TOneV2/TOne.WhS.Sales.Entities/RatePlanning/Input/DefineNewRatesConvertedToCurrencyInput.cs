using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class DefineNewRatesConvertedToCurrencyInput
    {
        public int CustomerId { get; set; }
        public int NewCurrencyId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public IEnumerable<int> NewCountryIds { get; set; }
    }
}
