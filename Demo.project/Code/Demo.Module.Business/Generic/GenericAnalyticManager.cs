using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Data;
using Demo.Module.Entities;
//using TOne.WhS.BusinessEntity.Business;


namespace Demo.Module.Business
{
    public class GenericAnalyticManager
    {
        private readonly IGenericAnalyticDataManager _gdatamanager;
        public GenericAnalyticManager()
        {
            _gdatamanager = DemoModuleDataManagerFactory.GetDataManager<IGenericAnalyticDataManager>();
        }

        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFiltered(Vanrise.Entities.DataRetrievalInput<GenericAnalyticQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _gdatamanager.GetAnalyticRecords(input));
        }
    }
}
