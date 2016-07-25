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

            var missingCDRBigResult = new MissingCDRBigResult();
            BigResult<MissingCDR> bigResult = dataManager.GetFilteredMissingCDRs(input);

            missingCDRBigResult.ResultKey = bigResult.ResultKey;
            missingCDRBigResult.Data = bigResult.Data;
            missingCDRBigResult.TotalCount = bigResult.TotalCount;

            missingCDRBigResult.Summary = new MissingCDR();
            missingCDRBigResult.Summary.DurationInSec = bigResult.Data.Sum(x => x.DurationInSec);

            return DataRetrievalManager.Instance.ProcessResult(input, missingCDRBigResult);
        }

        public int GetMissingCDRsCount(string tableKey, bool isPartnerCDRs)
        {
            IMissingCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetMissingCDRsCount(isPartnerCDRs);
        }

        public decimal GetDurationOfMissingCDRs(string tableKey, bool? isPartner)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IMissingCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDurationOfMissingCDRs(isPartner);
        }

        #endregion
    }
}
