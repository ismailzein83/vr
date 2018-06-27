using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Voucher.Entities;
using Vanrise.Common;
namespace Vanrise.Voucher.Business
{
    public class VoucherTypeManager
    {
        static Guid _definitionId = new Guid("73fa7201-b92b-4856-89f3-a38afb53323b");

        public VoucherType GetVoucherType(long voucherTypeId)
        {
            return GetCachedVoucherTypes().GetRecord(voucherTypeId);
        }
        private Dictionary<long,VoucherType>GetCachedVoucherTypes()
        {
            throw new NotImplementedException();
        }

    }
}
