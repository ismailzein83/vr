using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BI.Data;
using TOne.BI.Entities;

namespace TOne.BI.Business
{
    public class SalesManager
    {       
        public IEnumerable<ProfitInfo> GetProfit(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate)
        {
            ISalesDataManager dataManager = BIDataManagerFactory.GetDataManager<ISalesDataManager>();
            return dataManager.GetProfit(timeDimensionType, fromDate, toDate);
        }
    }
}
