using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
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


        public void SaveDWStrategiesToDB(List<DWStrategy> dwStrategies)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (DWStrategy strategy in dwStrategies)
            {
                WriteRecordToStream(strategy, dbApplyStream);
            }

            ApplyDWStrategiesToDB(FinishDBApplyStream(dbApplyStream));
        }

        public void ApplyDWStrategiesToDB(object preparedDWStrategies)
        {
            InsertBulkToTable(preparedDWStrategies as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[Dim_Strategy]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(DWStrategy record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}",
                                     record.Id
                                   , record.Name
                                   , record.Type
                                   , record.Kind
                                    );

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
