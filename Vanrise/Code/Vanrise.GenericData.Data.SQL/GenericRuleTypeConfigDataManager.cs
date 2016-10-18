using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.SQL
{
    public class GenericRuleTypeConfigDataManager:BaseSQLDataManager,IGenericRuleTypeConfigDataManager
    {
        //public GenericRuleTypeConfigDataManager()
        //    : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        //{

        //}

        //#region Public Methods
        //public List<GenericRuleTypeConfig> GetGenericRuleTypes()
        //{
        //    return GetItemsSP("genericdata.sp_GenericRuleTypeConfig_GetAll", GenericRuleTypeConfigMapper);
        //}
        //public bool AreGenericRuleTypeConfigUpdated(ref object updateHandle)
        //{
        //    return base.IsDataUpdated("genericdata.GenericRuleTypeConfig", ref updateHandle);
        //}
       
        //#endregion

        //#region Mappers

        //GenericRuleTypeConfig GenericRuleTypeConfigMapper(IDataReader reader)
        //{
        //    GenericRuleTypeConfig genericRuleTypeConfig = Vanrise.Common.Serializer.Deserialize<GenericRuleTypeConfig>(reader["Details"] as string);
        //    if (genericRuleTypeConfig != null)
        //    {
        //        genericRuleTypeConfig.GenericRuleTypeConfigId = Convert.ToInt32(reader["ID"]);
        //        genericRuleTypeConfig.Name = reader["Name"] as string;
        //    }
        //    return genericRuleTypeConfig;
        //}

        //#endregion
    }
}
