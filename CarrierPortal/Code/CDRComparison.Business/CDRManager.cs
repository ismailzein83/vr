using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace CDRComparison.Business
{
    public class CDRManager
    {
        public int GetAllCDRsCount()
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            return dataManager.GetAllCDRsCount();
        }
        public IDataRetrievalResult<CDR> GetFilteredCDRs(DataRetrievalInput<CDRQuery> input)
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            IEnumerable<CDR> cdrs = dataManager.GetCDRs(input.Query.IsPartnerCDRs,input.Query.CDPN);
            return DataRetrievalManager.Instance.ProcessResult(input, cdrs.ToBigResult(input, null));
        }
    }
}
