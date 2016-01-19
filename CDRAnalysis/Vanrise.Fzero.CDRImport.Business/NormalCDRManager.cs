using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Business
{
    public class NormalCDRManager
    {
        public IDataRetrievalResult<CDRDetail> GetNormalCDRs(DataRetrievalInput<NormalCDRQuery> input)
        {
            ICDRDataManager dataManager = CDRDataManagerFactory.GetDataManager<ICDRDataManager>();

            Vanrise.Entities.BigResult<CDR> cdrs = dataManager.GetNormalCDRs(input);
            Vanrise.Entities.BigResult<CDRDetail> cdrsBigResult = new BigResult<CDRDetail>();

            List<CDRDetail> listCDRDetails = new List<CDRDetail>();

            foreach (CDR cdr in cdrs.Data)
            {
                listCDRDetails.Add(NormalCDRDetailMapper(cdr));
            }
            cdrsBigResult.Data = listCDRDetails;
            cdrsBigResult.ResultKey = cdrs.ResultKey;
            cdrsBigResult.TotalCount = cdrs.TotalCount;




            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cdrsBigResult);
        }

          private CDRDetail NormalCDRDetailMapper(CDR cdr)
        {
            var normalCDR = new CDRDetail();
            normalCDR.Entity = cdr;

            //if (cdr.CallClassId != null)
            //{
            //    CallClass callClass = CallClassManager.GetCallClassById(cdr.CallClassId.Value);
            //    if (callClass != null)
            //        normalCDR.CallClassName = callClass.Description;
            //}

            normalCDR.CallTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.CallType);

            if (cdr.SubscriberType != null)
                normalCDR.SubscriberTypeName = Vanrise.Common.Utilities.GetEnumDescription(cdr.SubscriberType.Value);
            return normalCDR;
        }
    }
}
