using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class MappedCellContext:IMappedCellContext
    {
        public int CustomerId { get; set; }
        public DateTime PricelistDate { get; set; }
        public int PricelistCurrencyId { get; set; }
        public SalePriceListType PricelistType { get; set; }
        public object Value { get; set; }
    }
}
