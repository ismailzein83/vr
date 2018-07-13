using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RecurringChargeItem
    {
        public string Name { get; set; }
        public string NameDescription { get; set; }
        public string RecurringChargeIdDescription { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public decimal AmountAfterTaxes { get; set ; }
        public long RecurringChargeId { get; set; }
        public decimal VAT { get; set; }
        public IEnumerable<RecurringChargeItem> GetRecurringChargeItemRDLCSchema()
        {
            return null;
        }
    }
}
