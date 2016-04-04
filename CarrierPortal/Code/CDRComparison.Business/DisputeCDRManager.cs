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

        public IDataRetrievalResult<DisputeCDR> GetFilteredDisputeCDRs(DataRetrievalInput<DisputeCDRQuery> input)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;
            IEnumerable<DisputeCDR> disputeCDRs = dataManager.GetDisputeCDRs();
            return DataRetrievalManager.Instance.ProcessResult(input, disputeCDRs.ToBigResult(input, null));
        }

        public int GetDisputeCDRsCount(string tableKey)
        {
            IDisputeCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IDisputeCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDisputeCDRsCount();
        }

        #endregion
    }
}
