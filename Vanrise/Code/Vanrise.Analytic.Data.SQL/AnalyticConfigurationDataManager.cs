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
        public List<AnalyticConfiguration<MeasureConfiguration>> GetMeasures()
        {
            return GetItemsSP("Analytic.[SP_SchemaConfiguration_GetByType]", SchemaConfigurationMapper<MeasureConfiguration>, ConfigurationType.Measure);
        }

        public List<AnalyticConfiguration<DimensionConfiguration>> GetDimensions()
        {
            return GetItemsSP("Analytic.[SP_SchemaConfiguration_GetByType]", SchemaConfigurationMapper<DimensionConfiguration>, ConfigurationType.Dimension);
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
