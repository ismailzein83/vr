using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPriceListChange
    {
        public int CustomerId { get; set; }

        public int PriceListId { get; set; }


        private List<SalePricelistCodeChange> _codeChanges = new List<SalePricelistCodeChange>();
        public List<SalePricelistCodeChange> CodeChanges { get { return this._codeChanges; } }


        private List<SalePricelistRateChange> _rateChanges = new List<SalePricelistRateChange>();
        public List<SalePricelistRateChange> RateChanges { get { return this._rateChanges; } }
    }
}
