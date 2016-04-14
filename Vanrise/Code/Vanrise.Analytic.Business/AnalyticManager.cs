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

        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            ConfigurationManager configManager = new ConfigurationManager();
            var table = configManager.GetTable(input.Query.TableId);
            if (table == null)
                throw new NullReferenceException(String.Format("table '{0}'", input.Query.TableId));
            if (table.Settings == null)
                throw new NullReferenceException(String.Format("table.Settings '{0}'", input.Query.TableId));
            dataManager.Table = table;
            dataManager.Dimensions = configManager.GetDimensions(input.Query.TableId);
            dataManager.Measures = configManager.GetMeasures(input.Query.TableId);
            dataManager.Joins = configManager.GetJoins(input.Query.TableId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAnalyticRecords(input));
        }
    }
}
