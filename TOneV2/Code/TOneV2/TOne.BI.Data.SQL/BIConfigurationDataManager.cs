using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TOne.BI.Entities;
using TOne.Data.SQL;
namespace TOne.BI.Data.SQL
{
    class BIConfigurationDataManager : BaseTOneDataManager, IBIConfigurationDataManager
    {
        public BIConfigurationDataManager()
            : base(GetConnectionStringName("BIDBConnStringKey", "BIDBConnString"))
        {

        }

        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {

            return GetItemsSP("BI.SP_SchemaConfiguration_GetByType", SchemaConfigurationMapper<BIConfigurationMeasure>, ConfigurationType.Measure);
        }
        

        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {

            return GetItemsSP("BI.SP_SchemaConfiguration_GetByType", SchemaConfigurationMapper<BIConfigurationEntity>, ConfigurationType.Entity);
        }
        private BIConfiguration<T> SchemaConfigurationMapper<T>(IDataReader reader)
        {
            BIConfiguration<T> instance = new BIConfiguration<T>
            {
                Id = (int)reader["ID"],
                DisplayName = reader["DisplayName"] as string,
                Name = reader["Name"] as string,
                Type = (ConfigurationType)reader["Type"],
                Configuration = Vanrise.Common.Serializer.Deserialize<T>(reader["Configuration"] as string),
            };
            return instance;
        }

    }
}
