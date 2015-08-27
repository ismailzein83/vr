using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
   public class RateAnalysisManager
    {

       public Vanrise.Entities.IDataRetrievalResult<RateAnalysis> GetRateAnalysis(Vanrise.Entities.DataRetrievalInput<RateAnalysisQuery> input)
       {
           
               IRateAnalysisDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateAnalysisDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetRateAnalysis(input));

       }
    }
}
