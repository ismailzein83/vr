using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFiltered(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetAnalyticRecords(input));
        }
    }
}
