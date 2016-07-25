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

            var partialMatchCDRBigResult = new PartialMatchCDRBigResult();
            BigResult<PartialMatchCDR> bigResult = dataManager.GetFilteredPartialMatchCDRs(input);

            partialMatchCDRBigResult.ResultKey = bigResult.ResultKey;
            partialMatchCDRBigResult.Data = bigResult.Data;
            partialMatchCDRBigResult.TotalCount = bigResult.TotalCount;

            partialMatchCDRBigResult.Summary = new PartialMatchCDR();
            partialMatchCDRBigResult.Summary.SystemDurationInSec = 0;
            partialMatchCDRBigResult.Summary.PartnerDurationInSec = 0;
            partialMatchCDRBigResult.Summary.DurationDifferenceInSec = 0;

            foreach (PartialMatchCDR cdr in bigResult.Data)
            {
                cdr.DurationDifferenceInSec = Math.Abs(cdr.SystemDurationInSec - cdr.PartnerDurationInSec);

                partialMatchCDRBigResult.Summary.SystemDurationInSec += cdr.SystemDurationInSec;
                partialMatchCDRBigResult.Summary.PartnerDurationInSec += cdr.PartnerDurationInSec;
                partialMatchCDRBigResult.Summary.DurationDifferenceInSec += cdr.DurationDifferenceInSec;
            }

            return DataRetrievalManager.Instance.ProcessResult(input, partialMatchCDRBigResult);
        }

        public int GetPartialMatchCDRsCount(string tableKey)
        {
            IPartialMatchCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetPartialMatchCDRsCount();
        }

        public decimal GetDurationOfPartialMatchCDRs(string tableKey, bool isPartner)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetDurationOfPartialMatchCDRs(isPartner);
        }

        public decimal GetTotalDurationDifferenceOfPartialMatchCDRs(string tableKey)
        {
            var dataManager = CDRComparisonDataManagerFactory.GetDataManager<IPartialMatchCDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetTotalDurationDifferenceOfPartialMatchCDRs(tableKey);
        }

        #endregion
    }
}
