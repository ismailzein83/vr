using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;

namespace TOne.BusinessEntity.Business
{
    public class AccountManagerManager
    {
        public List<AccountManagerCarrier> GetCarriers(int from, int to)
        {
            IAccountManagerDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
            return dataManager.GetCarriers(from, to);
        }

        public List<AssignedAccountManagerCarrier> GetAssignedCarriers(int userId)
        {
            IAccountManagerDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
            return dataManager.GetAssignedCarriers(userId);
        }

        public void AssignCarriers(UpdatedAccountManagerCarrier[] updatedCarriers)
        {
            IAccountManagerDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountManagerDataManager>();
            dataManager.AssignCarriers(updatedCarriers);
        }
    }
}
