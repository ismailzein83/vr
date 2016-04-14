using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class ConfigurationManager
    {
        public AnalyticTable GetTable(int tableId)
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
            foreach(var itemConfig in GetAllItemConfigs().Where(itm => itm.TableId == tableId && itm.ItemType == AnalyticItemType.Measure))
            {
                AnalyticMeasureConfig measureConfig = itemConfig.Config as AnalyticMeasureConfig;
                if(measureConfig == null)
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

        public IEnumerable<T> GetItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            return GetAllItemConfigs().Select(itm => itm.Config as T);
        }

        public IEnumerable<AnalyticItemConfig> GetAllItemConfigs()
        {
            throw new NotImplementedException();
        }
    }
}
