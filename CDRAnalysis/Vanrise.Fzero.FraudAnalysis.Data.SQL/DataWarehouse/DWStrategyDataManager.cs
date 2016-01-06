using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class DWStrategyDataManager : BaseSQLDataManager, IDWStrategyDataManager
    {

        public DWStrategyDataManager()
            : base("DWSDBConnString")
        {

        }

        public List<DWStrategy> GetStrategies()
        {
            string query = string.Format("SELECT [Pk_StrategyId] ,[Name] ,[Type] ,[Kind] FROM [dbo].[Dim_Strategy]");
            return GetItemsText(query, DWStrategyMapper, (cmd) =>
            {
               
            });
        }


        #region Private Methods

        private DWStrategy DWStrategyMapper(IDataReader reader)
        {
            var dwStrategy = new DWStrategy();
            dwStrategy.Id = (int)reader["Pk_StrategyId"];
            dwStrategy.Name = reader["Name"] as string;
            dwStrategy.Type = reader["Type"] as string;
            dwStrategy.Kind = reader["Kind"] as string;
            return dwStrategy;
        }

        #endregion



    }
}
