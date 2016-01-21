using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Vanrise.BI.Entities;
using Vanrise.Data.SQL;
namespace Vanrise.BI.Data.SQL
{
    class BIConfigurationDataManager : BaseSQLDataManager, IBIConfigurationDataManager
    {

        #region ctor
        public BIConfigurationDataManager()
            : base(GetConnectionStringName("BIDBConnStringKey", "BIDBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<BIConfiguration<BIConfigurationMeasure>> GetMeasures()
        {
            return GetItemsSP("BI.SP_SchemaConfiguration_GetByType", SchemaConfigurationMapper<BIConfigurationMeasure>, ConfigurationType.Measure);
        }
        public List<BIConfiguration<BIConfigurationEntity>> GetEntities()
        {
            return GetItemsSP("BI.SP_SchemaConfiguration_GetByType", SchemaConfigurationMapper<BIConfigurationEntity>, ConfigurationType.Entity);
        }
        public List<BIConfiguration<BIConfigurationTimeEntity>> GetTimeEntities()
        {
            return GetItemsSP("BI.SP_SchemaConfiguration_GetByType", SchemaConfigurationMapper<BIConfigurationTimeEntity>, ConfigurationType.TimeEntity);
        }
        public bool AreBIConfigurationUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[BI].[SchemaConfiguration]", ref updateHandle);
        }
        #endregion

        #region Public Methods
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
        #endregion




   
    }
}
