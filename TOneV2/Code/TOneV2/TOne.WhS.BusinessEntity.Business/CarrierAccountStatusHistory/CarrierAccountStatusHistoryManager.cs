using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CarrierAccountStatusHistoryManager
    {
        public void AddAccountStatusHistory(int carrierAccountId, ActivationStatus status, ActivationStatus? previousStatus)
        {
            ICarrierAccountStatusHistoryDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountStatusHistoryDataManager>();
            dataManager.Insert(carrierAccountId, status, previousStatus);
        }
    }
}
