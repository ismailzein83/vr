using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RPQualityConfigurationDataManager : RoutingDataManager, IRPQualityConfigurationDataManager
    {
        string[] columns = { "ID", "Supplier", "Salezone", "Quality" };
        public void ApplyQualityConfigurationsToDB(object qualityConfigurations)
        {
            InsertBulkToTable(qualityConfigurations as StreamBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[QualityConfigurationData]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(RPQualityConfigurationData record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.QualityConfigurationId, record.SupplierId, record.SaleZoneId, record.QualityData);
        }
    }
}
