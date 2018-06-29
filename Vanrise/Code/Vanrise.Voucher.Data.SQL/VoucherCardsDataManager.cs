using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Voucher.Entities;


namespace Vanrise.Voucher.Data.SQL
{
    public class VoucherCardsDataManager : BaseSQLDataManager, IVoucherCardsDataManager
    {
        public VoucherCardsDataManager()
            : base(GetConnectionStringName("VoucherDBConnStringKey", "VoucherDBConnString"))
        {

        }

        public bool Update(SetVoucherUsedInput voucherUsedInput)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_VoucherUsed_Update]", voucherUsedInput.PinCode, voucherUsedInput.UsedBy);
            return (nbOfRecordsAffected > 0);
        }

    }
}
