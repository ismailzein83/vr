using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticConfigurationDataManager : BaseSQLDataManager, IAnalyticConfigurationDataManager
    {
        public Dictionary<string, AnalyticConfiguration<MeasureConfiguration>> GetMeasures()
        {
            Dictionary<string, AnalyticConfiguration<MeasureConfiguration>> measures = new Dictionary<string, AnalyticConfiguration<MeasureConfiguration>>();
            ExecuteReaderSP("Analytic.[SP_SchemaConfiguration_GetByType]", (reader) =>
            {
                while (reader.Read())
                {
                    AnalyticConfiguration<MeasureConfiguration> measure = SchemaConfigurationMapper<MeasureConfiguration>(reader);
                    measures.Add(measure.Name, measure);
                }
            }, ConfigurationType.Measure);
            return measures;
        }

        public Dictionary<string, AnalyticConfiguration<DimensionConfiguration>> GetDimensions()
        {
            Dictionary<string, AnalyticConfiguration<DimensionConfiguration>> dimensions = new Dictionary<string, AnalyticConfiguration<DimensionConfiguration>>();
            ExecuteReaderSP("Analytic.[SP_SchemaConfiguration_GetByType]", (reader) =>
            {
                while (reader.Read())
                {
                    AnalyticConfiguration<DimensionConfiguration> dimension = SchemaConfigurationMapper<DimensionConfiguration>(reader);
                    dimensions.Add(dimension.Name, dimension);
                }
            }, ConfigurationType.Dimension);
            return dimensions;
        }

        private AnalyticConfiguration<T> SchemaConfigurationMapper<T>(IDataReader reader)
        {
            AnalyticConfiguration<T> instance = new AnalyticConfiguration<T>
            {
                Id = (int)reader["ID"],
                Name = reader["Name"] as string,
                Type = (ConfigurationType)reader["Type"],
                Configuration = Vanrise.Common.Serializer.Deserialize<T>(reader["Configuration"] as string),
            };
            return instance;
        }
    }
}
