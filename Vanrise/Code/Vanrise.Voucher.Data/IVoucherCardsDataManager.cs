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
        bool Update(SetVoucherUsedInput voucherUsedInput);
    }
}
