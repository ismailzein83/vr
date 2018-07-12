using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
 public interface ICustomerBillingRecurringChargeDataManager : IDataManager
    {
        bool Insert(CustomerBillingRecurringCharge customerBilling);
    }
}
