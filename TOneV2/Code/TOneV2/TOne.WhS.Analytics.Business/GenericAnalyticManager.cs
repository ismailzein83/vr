using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.BusinessEntity.Business;


namespace TOne.WhS.Analytics.Business
{
    public class GenericAnalyticManager
    {
        private readonly IGenericAnalyticDataManager _gdatamanager;
        public GenericAnalyticManager()
        {
            _gdatamanager = AnalyticsDataManagerFactory.GetDataManager<IGenericAnalyticDataManager>();
        }

        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFiltered(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input)
        {
            return _gdatamanager.GetAnalyticRecords(input);
        }
    }
}
