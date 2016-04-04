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
    public class PartialMatchCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<PartialMatchCDR> GetFilteredPartialMatchCDRs(DataRetrievalInput<PartialMatchCDRQuery> input)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;
            IEnumerable<PartialMatchCDR> partialMatchCDRs = dataManager.GetPartialMatchCDRs();
            return DataRetrievalManager.Instance.ProcessResult(input, partialMatchCDRs.ToBigResult(input, null));
        }

        public int GetPartialMatchCDRsCount(string tableKey)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetPartialMatchCDRsCount();
        }

        #endregion
    }
}
