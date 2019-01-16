using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class SaleZoneRPPreviewDataManager : ISaleZoneRoutingProductPreviewDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhs_CP_SaleZoneRoutingProduct_Preview";
        static string TABLE_ALIAS = "rp";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_ZoneName = "ZoneName";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_ChangeType = "ChangeType";


        static SaleZoneRPPreviewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ZoneName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ChangeType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhs_CP",
                DBTableName = "SaleZoneRoutingProduct_Preview",
                Columns = columns
            });
        }

        #endregion
        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_CodePrep", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Mappers
        private ZoneRoutingProductPreview ZoneRoutingProductPreviewMapper(IRDBDataReader reader)
        {
            ZoneRoutingProductPreview zoneRoutingProductPreview = new ZoneRoutingProductPreview
            {
                ZoneName = reader.GetString(COL_ZoneName),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                OwnerId = reader.GetInt(COL_OwnerID),
                RoutingProductId = reader.GetInt(COL_RoutingProductID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                ChangeType = (ZoneRoutingProductChangeType)reader.GetInt(COL_ChangeType),
            };
            return zoneRoutingProductPreview;
        }
        #endregion
        #region ISaleZoneRoutingProductPreviewDataManager
        readonly string[] _columns = { COL_ProcessInstanceID, COL_ZoneName, COL_OwnerType, COL_OwnerID, COL_RoutingProductID, COL_BED, COL_EED, COL_ChangeType };
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }

        long _processInstanceID;
        public void ApplyPreviewZonesRoutingProductsToDB(object preparedRates)
        {
            preparedRates.CastWithValidate<RDBBulkInsertQueryContext>("preparedRates").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public IEnumerable<ZoneRoutingProductPreview> GetFilteredZonesRoutingProductsPreview(SPLPreviewQuery query) //this method is not used anymore and hence was not tested
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            var where = selectQuery.Where();
            if (query.ZoneName != null)
                where.EqualsCondition(COL_ZoneName).Value(query.ZoneName);
            where.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);
            if (!query.OnlyModified)
                where.NotEqualsCondition(COL_ChangeType).Value((int)ZoneRoutingProductChangeType.NotChanged);
            return queryContext.GetItems(ZoneRoutingProductPreviewMapper);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(ZoneRoutingProductPreview record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.ZoneName);
            recordContext.Value((int)record.OwnerType);
            recordContext.Value(record.OwnerId);
            recordContext.Value(record.RoutingProductId);
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
            recordContext.Value((int)record.ChangeType);
        }
        #endregion

        #region CodePreparation
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }
        #endregion
    }
}
