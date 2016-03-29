using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Data
{
    public interface ICDRDataManager:IDataManager
    {
        void LoadCDRs(Action<CDR> onBatchReady);
    }
}
