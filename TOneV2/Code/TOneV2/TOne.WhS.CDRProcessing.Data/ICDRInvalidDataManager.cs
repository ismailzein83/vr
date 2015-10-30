using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CDRProcessing.Entities;

namespace TOne.WhS.CDRProcessing.Data
{
   public interface ICDRInvalidDataManager:IDataManager
    {
       void SaveInvalidCDRBatchToDB(CDRInvalidBatch cdrBatch);
    }
}
