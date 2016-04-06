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
            return DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredDisputeCDRs(input));
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
