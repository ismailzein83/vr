using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class GenericConfigurationDataManager : BaseSQLDataManager, IGenericConfigurationDataManager
    {
        public GenericConfigurationDataManager(): base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
        }

        public bool UpdateConfiguration(string ownerKey, int typeId, GenericConfiguration genericConfig)
        {
            string serializedObject=null;
            if(genericConfig!=null)
                 serializedObject = Serializer.Serialize(genericConfig);

            int recordesEffected = ExecuteNonQuerySP("common.sp_GenericConfiguration_Update", ownerKey, typeId, serializedObject);
            return recordesEffected > 0;
        }

        public Dictionary<string, GenericConfiguration> GetALllConfigurations()
        {

            Dictionary<string, GenericConfiguration> instance = new Dictionary<string, GenericConfiguration>();
            ExecuteReaderSP("common.sp_GenericConfiguration_GetAll", (reader) =>
            {
                string key=null;
                while (reader.Read())
                {
                    key = String.Format("{0}_{1}", reader["OwnerKey"] as string, GetReaderValue<int>(reader, "TypeID"));
                    if (!instance.ContainsKey(key))
                    {
                        instance.Add(key, Serializer.Deserialize<GenericConfiguration>(reader["ConfigDetails"] as string));
                    }
                    
                }
            });
            return instance;
        }
        public bool AreGenericConfigurationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[GenericConfiguration]", ref updateHandle);
        }

    }
}
