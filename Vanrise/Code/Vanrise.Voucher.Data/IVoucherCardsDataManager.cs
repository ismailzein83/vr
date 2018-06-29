using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Voucher.Entities;

namespace Vanrise.Voucher.Data
{
    public interface IVoucherCardsDataManager : IDataManager
    {
        bool SetVoucherUsed(string pinCode, string usedBy, int lastModifiedBy);
        bool SetVoucherLocked(string pinCode, string lockedBy, int lastModifiedBy);

    }
}
