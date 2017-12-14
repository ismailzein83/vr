using Vanrise.Data.SQL;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerQualityConfigurationDataManager : RoutingDataManager, ICustomerQualityConfigurationDataManager
    {
        string[] columns = { "ID", "SupplierZone", "Quality" };
        public void ApplyQualityConfigurationsToDB(object qualityConfigurations)
        {
            InsertBulkToTable(qualityConfigurations as BaseBulkInsertInfo);
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

        public void WriteRecordToStream(CustomerRouteQualityConfigurationData record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}", record.QualityConfigurationId, record.SupplierZoneId, record.QualityData);
        }
    }
}
