using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CodeSaleZoneDataManager : RoutingDataManager, ICodeSaleZoneDataManager
    {
        readonly string[] columns = { "Code", "SaleZoneId" };

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(Entities.CodeSaleZone record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}", record.Code, record.SaleZoneId);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CodeSaleZone]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public void ApplyCodeToCodeSaleZoneTable(object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BaseBulkInsertInfo);
        }
    }
}