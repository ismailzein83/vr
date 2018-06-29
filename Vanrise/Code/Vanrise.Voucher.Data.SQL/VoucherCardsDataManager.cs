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

        public bool SetVoucherUsed(string pinCode, string usedBy, int lastModifiedBy)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[Voucher].[sp_VoucherCards_SetUsed]", pinCode, usedBy, lastModifiedBy);
            return (nbOfRecordsAffected > 0);
        }

        public bool SetVoucherLocked(string pinCode, string lockedBy, int lastModifiedBy)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[Voucher].[sp_VoucherCards_SetLocked]", pinCode, lockedBy, lastModifiedBy);
            return (nbOfRecordsAffected > 0);
        }
    }
}
