using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Data;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Business
{
    public class CDRInvalidManager
    {
        public void SaveInvalidCDRBatchToDB(CDRInvalidBatch cdrInvalidBatch)
        {
            ICDRInvalidDataManager dataManager = CDRProcessingDataManagerFactory.GetDataManager<ICDRInvalidDataManager>();
            dataManager.SaveInvalidCDRBatchToDB(cdrInvalidBatch);
        }
    }
}
