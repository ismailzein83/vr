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
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFiltered(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetAnalyticRecords(input));
        }
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            ConfigurationManager configManager = new ConfigurationManager();
            var table = configManager.GetTableByName(input.Query.TableName);
            if (table == null)
                throw new NullReferenceException(String.Format("table. Name '{0}'", input.Query.TableName));
            if (table.Settings == null)
                throw new NullReferenceException(String.Format("table.Settings. ID '{0}'", table.AnalyticTableId));
            dataManager.Table = table;
            dataManager.Dimensions = configManager.GetDimensions(table.AnalyticTableId);
            dataManager.Measures = configManager.GetMeasures(table.AnalyticTableId);
            dataManager.Joins = configManager.GetJoins(table.AnalyticTableId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredAnalyticRecords(input));
        }

        public IEnumerable<TemplateConfig> GetAnalyticReportSettingsTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.AnalyticReportSettingsConfigType);
        }
        public IEnumerable<TemplateConfig> GetWidgetsTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.AnalyticWidgetsConfigType);
        }
    }
}
