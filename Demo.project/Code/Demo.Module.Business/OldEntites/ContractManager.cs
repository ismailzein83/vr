using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class ContractManager
    {

        #region Public Methods
        public Contract GetContract()
        {
            IContractDataManager contractDataManager = DemoModuleFactory.GetDataManager<IContractDataManager>();
            return contractDataManager.GetContract();
        }
        #endregion
    }
}
