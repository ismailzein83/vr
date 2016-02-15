using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class CDRManager
    {
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
            var normalCDR = new CDRDetail();
            normalCDR.Entity = cdr;

            normalCDR.CallTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.CallType);

            if (cdr.SubscriberType != null)
                normalCDR.SubscriberTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.SubscriberType.Value);
            return normalCDR;
        }
    }
}
