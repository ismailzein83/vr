using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;


namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierBillingRecurringChargeManager
    {
        public InsertOperationOutput<SupplierBillingRecurringCharge> AddSupplierBillingRecurringCharge(SupplierBillingRecurringCharge supplierBilling)
        {
            ISupplierBillingRecurringChargeDataManager supplierBillingDataManager = BEDataManagerFactory.GetDataManager<ISupplierBillingRecurringChargeDataManager>();
            InsertOperationOutput<SupplierBillingRecurringCharge> insertOperationOutput = new InsertOperationOutput<SupplierBillingRecurringCharge>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            bool insertActionSucc = supplierBillingDataManager.Insert(supplierBilling);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = supplierBilling;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
    }
}
