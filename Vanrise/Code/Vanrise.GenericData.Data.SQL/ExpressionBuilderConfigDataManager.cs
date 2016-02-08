using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities.ExpressionBuilder;

namespace Vanrise.GenericData.Data.SQL
{
    public class ExpressionBuilderConfigDataManager:BaseSQLDataManager,IExpressionBuilderConfigDataManager
    {
        public ExpressionBuilderConfigDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey"))
        {

        }
       
        #region Public Methods
        public List<Entities.ExpressionBuilder.ExpressionBuilderConfig> GetExpressionBuilderTemplates()
        {
            return GetItemsSP("genericdata.sp_ExpressionBuilderConfig_GetAll", ExpressionBuilderConfigMapper);

        }
        public bool AreExpressionBuilderConfigUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("genericdata.ExpressionBuilderConfig", ref updateHandle);
        }

        #endregion

        #region Mappers

        ExpressionBuilderConfig ExpressionBuilderConfigMapper(IDataReader reader)
        {
            ExpressionBuilderConfig expressionBuilderConfig = Vanrise.Common.Serializer.Deserialize<ExpressionBuilderConfig>(reader["Details"] as string);
            if (expressionBuilderConfig != null)
            {
                expressionBuilderConfig.ExpressionBuilderConfigId = Convert.ToInt32(reader["ID"]);
                expressionBuilderConfig.Name = reader["Name"] as string;
            }
            return expressionBuilderConfig;
        }

        #endregion
       


    }
}
