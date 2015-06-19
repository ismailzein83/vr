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
        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {

            return GetItemsSP("BI.SP_SchemaConfiguration_GetDataByType", MeasuresMapper, ConfigurationType.Measure);
        }
        private BIConfiguration<BIConfigurationMeasure> MeasuresMapper(IDataReader reader)
        {
          
            BIConfiguration<BIConfigurationMeasure> instance = new BIConfiguration<BIConfigurationMeasure>
            {
                Id = GetReaderValue<Int32>(reader, "ID"),
                Name = reader["Name"] as string,
                Type=reader["Type"] as string,
               Configuration =Vanrise.Common.Serializer.Deserialize<BIConfigurationMeasure>(reader["Configuration"]  as string),
                //Configuration=reader["Configuration"] as string,
            };
            return instance;
        }

        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {

            return GetItemsSP("BI.SP_SchemaConfiguration_GetDataByType", EntitiesMapper, ConfigurationType.Entity);
        }
        private BIConfiguration<BIConfigurationEntity> EntitiesMapper(IDataReader reader)
        {
            BIConfiguration<BIConfigurationEntity> instance = new BIConfiguration<BIConfigurationEntity>
            {
                Id = GetReaderValue<Int32>(reader, "ID"),
                Name = reader["Name"] as string,
                Type = reader["Type"] as string,
               Configuration = Vanrise.Common.Serializer.Deserialize<BIConfigurationEntity>(reader["Configuration"] as string),
            };
            return instance;
        }

    }
}
