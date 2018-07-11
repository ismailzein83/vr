using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class PaymentManager
    {

        #region Public Methods
        public List<Payment> GetPayments()
        {
            IPaymentDataManager paymentDataManager = DemoModuleFactory.GetDataManager<IPaymentDataManager>();
            return paymentDataManager.GetPayments();
        }
        #endregion
    }
}
