using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class ServiceManager
    {
        IServiceDataManager _dataManager;
        public ServiceManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IServiceDataManager>();
        }

        public List<FlaggedService> GetServiceFlag()
        {
            return _dataManager.GetServiceFlag();
        }
    }
}
