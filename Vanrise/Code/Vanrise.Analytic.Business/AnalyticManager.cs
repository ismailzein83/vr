using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            AnalyticTableManager analyticTableManager = new AnalyticTableManager();
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            var table = analyticTableManager.GetAnalyticTableById(input.Query.TableId);
            if (table == null)
                throw new NullReferenceException(String.Format("table.ID '{0}'", input.Query.TableId));
            if (table.Settings == null)
                throw new NullReferenceException(String.Format("table.Settings.ID '{0}'", table.AnalyticTableId));
            dataManager.Table = table;
            dataManager.Dimensions = analyticItemConfigManager.GetDimensions(table.AnalyticTableId);
            dataManager.Measures = analyticItemConfigManager.GetMeasures(table.AnalyticTableId);
            dataManager.Joins = analyticItemConfigManager.GetJoins(table.AnalyticTableId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAnalyticRecords(input));
        }
    }
}
