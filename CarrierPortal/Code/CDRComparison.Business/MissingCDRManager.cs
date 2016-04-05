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
    public class MissingCDRManager
    {
        #region Public Methods

        public IDataRetrievalResult<MissingCDR> GetFilteredMissingCDRs(DataRetrievalInput<MissingCDRQuery> input)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;
            IEnumerable<MissingCDR> missingCDRs = dataManager.GetMissingCDRs(input.Query.IsPartnerCDRs);
            return DataRetrievalManager.Instance.ProcessResult(input, missingCDRs.ToBigResult(input, null));
        }

        public int GetMissingCDRsCount(string tableKey,bool isPartnerCDRs)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetMissingCDRsCount(isPartnerCDRs);
        }

        #endregion
    }
}
