
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;

namespace TOne.Analytics.Business
{
   public class HourlyReportManager
    {
       public Vanrise.Entities.IDataRetrievalResult<string> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<string> input)
       {
           IHourlyReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IHourlyReportDataManager>();
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetHourlyReportData(input));
       }
    }
}
