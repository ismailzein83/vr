using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticConfigurationManager
    {
        public AnalyticConfigurationManager()
        {

        }

        public Dictionary<string, DimensionConfiguration> GetDimensions(IEnumerable<string> filteredDimensions)
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            var dimensions = dataManager.GetDimensions();
            Dictionary<string, DimensionConfiguration> result = new Dictionary<string, DimensionConfiguration>();
            foreach (string itm in filteredDimensions)
            {
                var dimension = dimensions[itm];
                if (!result.ContainsKey(itm))
                    result.Add(itm, dimension.Configuration);
            }
            return result;
        }

        public Dictionary<string, MeasureConfiguration> GetMeasures(IEnumerable<string> filteredMeasures)
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            var measures = dataManager.GetMeasures();
            Dictionary<string, MeasureConfiguration> result = new Dictionary<string, MeasureConfiguration>();
            foreach (string itm in filteredMeasures)
            {
                var measure = measures[itm];
                if (!result.ContainsKey(itm))
                    result.Add(itm, measure.Configuration);
            }
            return result;
        }

        public IEnumerable<DimensionConfiguration> GetDimensions()
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            return dataManager.GetDimensions().Values.Select(s => s.Configuration);
        }

        public IEnumerable<MeasureConfiguration> GetMeasures()
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            return dataManager.GetMeasures().Values.Select(s => s.Configuration);
        }
    }
}
