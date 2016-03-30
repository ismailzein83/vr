using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class DisputeCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<DisputeCDR> GetFilteredDisputeCDRs(DataRetrievalInput<object> input)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            IEnumerable<DisputeCDR> disputeCDRs = dataManager.GetDisputeCDRs();
            return DataRetrievalManager.Instance.ProcessResult(input, disputeCDRs.ToBigResult(input, null));
        }

        public int GetDisputeCDRsCount()
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            return dataManager.GetDisputeCDRsCount();
        }

        #endregion
    }
}
