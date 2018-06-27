using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.Voucher.Business
{
    public class ActivateVouchersAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A0677B2C-4D1D-493B-A1BB-50940034E25E"); }
        }
        public override string ActionTypeName { get { return "ActivateVouchers"; } }

    }
}
