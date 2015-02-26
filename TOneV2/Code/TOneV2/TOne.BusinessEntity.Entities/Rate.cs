using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Rate
    {
        public long RateId { get; set; }

        public int ZoneId { get; set; }

        public int PriceListId { get; set; }

        public string CustomerId { get; set; }

        public string SupplierId { get; set; }

        public decimal NormalRate { get; set; }

        public decimal OffPeakRate { get; set; }

        public decimal WeekendRate { get; set; }

        public short ServicesFlag { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime EndEffectiveDate { get; set; }

        public string CurrencyID { get; set; }

        public float CurrencyLastRate { get; set; }

        public Change Change { get; set; }

    }
}
