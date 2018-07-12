using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Data.SQL;
using System.Data;
using System.Security;
using TOne.WhS.BusinessEntity.Entities;


namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerBillingRecurringChargeDataManager : BaseSQLDataManager, TOne.WhS.BusinessEntity.Business.ICustomerBillingRecurringChargeDataManager
    {

          #region ctor/Local Variables
        public CustomerBillingRecurringChargeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        public bool Insert(CustomerBillingRecurringCharge customerBillingRecurringCharge)
        {


            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerBillingRecurringCharge_Insert", customerBillingRecurringCharge.FinancialAccountId, customerBillingRecurringCharge.RecurringChargeId, customerBillingRecurringCharge.InvoiceId, customerBillingRecurringCharge.Amount, customerBillingRecurringCharge.From, customerBillingRecurringCharge.To, customerBillingRecurringCharge.CurrencyId, customerBillingRecurringCharge.CreatedBy,customerBillingRecurringCharge.VAT);
            bool insertedSuccesfully = (recordsEffected > 0);
            return (insertedSuccesfully);
        }

    }
}
