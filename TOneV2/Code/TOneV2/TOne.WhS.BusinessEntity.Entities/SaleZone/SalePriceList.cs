using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SalePriceListOwnerType : byte
    {
        [Description("Selling Product")]
        SellingProduct = 0,
        [Description("Customer")]
        Customer = 1
    }
    public class SalePriceList
    {
        public int PriceListId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int CurrencyId { get; set; }
    }
}
