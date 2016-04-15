using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticConfigurationDataManager : BaseSQLDataManager, IAnalyticConfigurationDataManager
    {
        public AnalyticConfigurationDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        public Dictionary<string, DimensionConfiguration> GetFilteredDimensions(IEnumerable<string> filteredDimensions)
        {
            var dimensions = GetDimensions();
            Dictionary<string, DimensionConfiguration> result = new Dictionary<string, DimensionConfiguration>();
            foreach (string dimensionName in filteredDimensions)
            {
                var dimension = dimensions.FindRecord(d => d.Name == dimensionName);
                if (!result.ContainsKey(dimensionName))
                    result.Add(dimensionName, dimension.Configuration);
            }
            return result;
        }
        public Dictionary<string, MeasureConfiguration> GetFilteredMeasures(IEnumerable<string> filteredMeasures)
        {
            var measures = GetMeasures();
            Dictionary<string, MeasureConfiguration> result = new Dictionary<string, MeasureConfiguration>();
            foreach (string measureName in filteredMeasures)
            {
                var measure = measures.FindRecord(m => m.Name == measureName);
                if (!result.ContainsKey(measureName))
                    result.Add(measureName, measure.Configuration);
            }
            return result;
        }
        public IEnumerable<AnalyticConfiguration<MeasureConfiguration>> GetMeasures()
        {

            return GetItemsSP("Analytic.[SP_SchemaConfiguration_GetByType]", SchemaConfigurationMapper<MeasureConfiguration>, ConfigurationType.Measure);

        }

        public IEnumerable<AnalyticConfiguration<DimensionConfiguration>> GetDimensions()
        {

            return GetItemsSP("Analytic.[SP_SchemaConfiguration_GetByType]", SchemaConfigurationMapper<DimensionConfiguration>, ConfigurationType.Dimension);

        }

        private AnalyticConfiguration<T> SchemaConfigurationMapper<T>(IDataReader reader)
        {
            AnalyticConfiguration<T> instance = new AnalyticConfiguration<T>
            {
                Id = (int)reader["ID"],
                Name = reader["Name"] as string,
                DisplayName = reader["DisplayName"] as string,
                Type = (ConfigurationType)reader["Type"],
                Configuration = Vanrise.Common.Serializer.Deserialize<T>(reader["Configuration"] as string),
            };
            return instance;
        }


        public bool AreAnalyticConfigurationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[SchemaConfiguration]", ref updateHandle);
        }
    }
}
