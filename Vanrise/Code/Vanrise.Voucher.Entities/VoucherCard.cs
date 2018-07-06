using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public class VoucherCard
    {
        public long GenerationVoucherId { get; set; }
        public string SerialNumber { get; set; }
        public long VoucherTypeId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public long VoucherCardsId { get; set; }        
        public DateTime ExpiryDate { get; set; }
        public DateTime UsedDate { get; set; }
        public DateTime ActivationDate { get; set; }

    }
}
