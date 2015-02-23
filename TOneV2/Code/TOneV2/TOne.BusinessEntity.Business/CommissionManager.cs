using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CommissionManager
    {
        ICommissionDataManager _commissionManager;

        public CommissionManager()
        {
            _commissionManager = BEDataManagerFactory.GetDataManager<ICommissionDataManager>();
        }
        public  List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            return _commissionManager.GetCommission( customerId,  zoneId,  when);
        }
    }
}
