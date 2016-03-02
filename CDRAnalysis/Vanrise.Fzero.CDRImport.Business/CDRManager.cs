using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class CDRManager
    {
        public IEnumerable<string> GetNumberPrefixes(DateTime fromTime, DateTime toTime)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();
            return dataManager.GetNumberPrefixes(fromTime, toTime);
        }

        public IDataRetrievalResult<CDRDetail> GetCDRs(DataRetrievalInput<CDRQuery> input)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();

            Vanrise.Entities.BigResult<CDR> cdrs = dataManager.GetCDRs(input);
            Vanrise.Entities.BigResult<CDRDetail> cdrsBigResult = new BigResult<CDRDetail>();

            List<CDRDetail> listCDRDetails = new List<CDRDetail>();

            foreach (CDR cdr in cdrs.Data)
            {
                listCDRDetails.Add(CDRDetailMapper(cdr));
            }
            cdrsBigResult.Data = listCDRDetails;
            cdrsBigResult.ResultKey = cdrs.ResultKey;
            cdrsBigResult.TotalCount = cdrs.TotalCount;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cdrsBigResult);
        }

          private CDRDetail CDRDetailMapper(CDR cdr)
        {
            var cdrDetail = new CDRDetail();
            cdrDetail.Entity = cdr;

            cdrDetail.CallTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.CallType);

            if (cdr.SubscriberType != null)
                cdrDetail.SubscriberTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.SubscriberType.Value);
            return cdrDetail;
        }
    }
}
