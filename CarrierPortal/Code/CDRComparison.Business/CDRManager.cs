using CDRComparison.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Business
{
    public class CDRManager
    {
        public int GetAllCDRsCount()
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            return dataManager.GetAllCDRsCount();
        }

    }
}
