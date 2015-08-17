
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
   public class HourlyReportManager
    {
       public Vanrise.Entities.IDataRetrievalResult<GroupSummary<HourlyReport>> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<HourlyReportInput> input)
       {
           IHourlyReportDataManager dataManager = AnalyticsDataManagerFactory.GetDataManager<IHourlyReportDataManager>();
           return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetHourlyReportData(input));
       }
    }
}
