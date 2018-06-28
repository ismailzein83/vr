using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Voucher.Entities
{
    public enum SetVoucherUsedResult { Succeeded = 0, Failed = 1 }
    public class SetVoucherUsedOutput
    {
        public SetVoucherUsedResult Result { get; set; }
    }
    public class SetVoucherUsedInput
    {
        public string PinCode { get; set; }
        public string UsedBy { get; set; }
    }
}
