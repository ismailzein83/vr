using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Business
{
    public class ServiceManager
    {

        #region Public Methods
        public List<Service> GetServices()
        {
            IServiceDataManager serviceDataManager = DemoModuleFactory.GetDataManager<IServiceDataManager>();
            return serviceDataManager.GetServices();
        }
        #endregion
    }
}
