using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class RepeatedNumbersManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RepeatedNumbers> GetRepeatedNumbersData(Vanrise.Entities.DataRetrievalInput<RepeatedNumbersInput> input)
        {
            IRepeatedNumbersDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IRepeatedNumbersDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetRepeatedNumbersData(input));
        }
    }
}
