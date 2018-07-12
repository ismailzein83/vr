using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class CustomerBillingRecurringChargeManager
    {
       public InsertOperationOutput<CustomerBillingRecurringCharge> AddCustomerBillingRecurringCharge(CustomerBillingRecurringCharge customerBilling)
        {
            ICustomerBillingRecurringChargeDataManager customerBillingDataManager = BEDataManagerFactory.GetDataManager<ICustomerBillingRecurringChargeDataManager>();
            InsertOperationOutput<CustomerBillingRecurringCharge> insertOperationOutput = new InsertOperationOutput<CustomerBillingRecurringCharge>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            bool insertActionSucc = customerBillingDataManager.Insert(customerBilling);
            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = customerBilling;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
    }
}
