using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;

namespace Retail.RA.Business
{
    public class RetailReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        #region MemoryAnalyticDataManager
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
