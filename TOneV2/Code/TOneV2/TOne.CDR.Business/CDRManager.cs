using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Data;

namespace TOne.CDR.Business
{
    public class CDRManager
    {
        public void LoadCDRRange(DateTime from, DateTime to, int? batchSize, Action<System.Collections.Generic.List<TABS.CDR>> onBatchReady)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.LoadCDRRange(from,to,batchSize,onBatchReady);
        }

        public Object PrepareCDRsForDBApply(System.Collections.Generic.List<TABS.CDR> cdrs, int SwitchID)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
           return dataManager.PrepareCDRsForDBApply(cdrs, SwitchID);
        }

       public void ApplyCDRsToDB(Object preparedCDRs)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.ApplyCDRsToDB(preparedCDRs);
        }
    }
}
