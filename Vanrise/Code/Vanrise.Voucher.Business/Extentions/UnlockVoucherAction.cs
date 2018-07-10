using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Voucher.Business
{
    public class UnlockVoucherAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("71AD4B25-D3E7-47BF-BE5F-87623448263D"); }
        }
        public override string ActionTypeName { get { return "UnlockVoucher"; } }

    }
}
