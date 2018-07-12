using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierBillingRecurringChargeDataManager : BaseSQLDataManager, TOne.WhS.BusinessEntity.Data.ISupplierBillingRecurringChargeDataManager
    {
                  #region ctor/Local Variables
        public SupplierBillingRecurringChargeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        public bool Insert(SupplierBillingRecurringCharge supplierBillingRecurringCharge)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierBillingRecurringCharge_Insert", supplierBillingRecurringCharge.FinancialAccountId, supplierBillingRecurringCharge.RecurringChargeId, supplierBillingRecurringCharge.InvoiceId, supplierBillingRecurringCharge.Amount, supplierBillingRecurringCharge.From, supplierBillingRecurringCharge.To, supplierBillingRecurringCharge.CurrencyId, supplierBillingRecurringCharge.CreatedBy, supplierBillingRecurringCharge.VAT);
            bool insertedSuccesfully = (recordsEffected > 0);
            return (insertedSuccesfully);
        }

    }
}
