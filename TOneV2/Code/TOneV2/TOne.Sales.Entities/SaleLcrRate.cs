using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Sales.Entities
{
    public class SaleLcrRate
    {
        public int ZoneId { get; set; }
        public int PriceListId { get; set; }
        public decimal OldRate { get; set; }
        public decimal NewRate { get; set; }
        public short ServiceFlag { get; set; }
    }
}
