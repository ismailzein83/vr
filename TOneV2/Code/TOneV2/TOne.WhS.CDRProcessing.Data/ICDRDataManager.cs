using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
    public interface ICDRDataManager:IDataManager
    {
        object InitialiazeStreamForDBApply();
        void WriteRecordToStream(CDR record, object dbApplyStream);
        void ApplyRawCDRsToDB(Object preparedCDRs);
        object FinishDBApplyStream(object dbApplyStream);
        void SaveCDRBatchToDB(CDRBatch cdrBatch);
       
    }
}
