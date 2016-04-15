using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class ConfigurationManager
    {
        public AnalyticTable GetTable(int tableId)
        {
            throw new NotImplementedException();
        }

        internal AnalyticTable GetTableByName(string tableName)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, AnalyticDimension> GetDimensions(int tableId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, AnalyticMeasure> GetMeasures(int tableId)
        {
            var measureConfigs = GetItemConfigs<AnalyticMeasureConfig>(tableId, AnalyticItemType.Measure);
            Dictionary<string, AnalyticMeasure> analyticMeasures = new Dictionary<string, AnalyticMeasure>();
            foreach (var itemConfig in measureConfigs)
            {
                AnalyticMeasureConfig measureConfig = itemConfig.Config;
                if (measureConfig == null)
                    throw new NullReferenceException("measureConfig");
                AnalyticMeasure measure = new AnalyticMeasure
                {
                    AnalyticMeasureConfigId = itemConfig.AnalyticItemConfigId,
                    Config = measureConfig,
                    Evaluator = DynamicTypeGenerator.GetMeasureEvaluator(itemConfig.AnalyticItemConfigId)
                };
                analyticMeasures.Add(itemConfig.Name, measure);
            }
            return analyticMeasures;
        }

        public Dictionary<string, AnalyticJoin> GetJoins(int tableId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AnalyticItemConfig<T>> GetItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            return dataManager.GetItemConfigs<T>(tableId, itemType);
        }

        //public IEnumerable<AnalyticItemConfig> GetAllItemConfigs()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
