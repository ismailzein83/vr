using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public class VoucherType
    {
        public long VoucherTypeId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
    }
}
