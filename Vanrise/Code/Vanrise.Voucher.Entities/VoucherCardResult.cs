using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public enum VoucherCardOperationResult { Success = 0 , Failed = 1}
    public class VoucherCardResult
    {
        public VoucherCardOperationResult Result { get; set; }
        public VoucherCard EntityResult { get; set; }
    }
}
