using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class PriceList
    {
        public int PriceListID { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string CurrencyId { get; set; }
        public Nullable<DateTime> EndEffectiveDate { get; set; }
        public Nullable<DateTime> BeginEffectiveDate { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public int UserId { get; set; }
        public bool IsSend { get; set; }
        public string SourceFileName { get; set; }
    }
}
