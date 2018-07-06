using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public class VoucherCardsGeneration
    {
        public long VoucherCardsGenerationId { get; set; }
        public string Name { get; set; }
        public long VoucherTypeId { get; set; }
        public int LastModifiedBy  { get; set; }
        public int NumberOfCards { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int? InactiveCards { get; set; }
    }
}
