using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class Commission
    {
        public int ID { get; set; }
        public int SupplierId { get; set; }
        public int CustomerId { get; set; }
        public int ZoneId { get; set; }
        public float? FromRate { get; set; }
        public float? ToRate { get; set; }
        public float? Percentage { get; set; }
        public decimal? Amount { get; set; }
        public DateTime BED { get; set; }
        public DateTime EED { get; set; }
        public bool IsExtraCharge { get; set; }
        public bool IsEffective { get; set; }
        public int UserId { get; set; }
    }
}
