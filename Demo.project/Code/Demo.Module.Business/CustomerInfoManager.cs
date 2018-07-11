using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class CustomerInfoManager
    {

        #region Public Methods
        public CustomerInfo GetCustomerInfo()
        {
            ICustomerInfoDataManager customerInfoDataManager = DemoModuleFactory.GetDataManager<ICustomerInfoDataManager>();
            return customerInfoDataManager.GetCustomerInfo();
        }
        #endregion
    }
}
