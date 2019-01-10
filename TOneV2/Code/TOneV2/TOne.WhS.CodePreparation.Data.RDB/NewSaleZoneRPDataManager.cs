using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Data.RDB;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class NewSaleZoneRPDataManager : INewSaleZoneRoutingProductDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_CP_SaleZoneRoutingProduct_New";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_ZoneID = "ZoneID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static NewSaleZoneRPDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CP_SaleZoneRoutingProduct_New",
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

        #region INewSaleZoneRoutingProductDataManager
        public long ProcessInstanceId
        {
            set
            {
                _processInstanceID = value;
            }
        }
        long _processInstanceID;
        readonly string[] _columns = { COL_ID, COL_ProcessInstanceID, COL_ZoneID, COL_OwnerType, COL_OwnerID, COL_RoutingProductID, COL_BED, COL_EED };

        public void ApplyNewZonesRoutingProductsToDB(object preparedRates)
        {
            preparedRates.CastWithValidate<RDBBulkInsertQueryContext>("preparedRates").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', _columns);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(AddedZoneRoutingProduct record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.SaleEntityRoutingProductId);
            recordContext.Value(_processInstanceID);
            recordContext.Value(record.AddedZone.ZoneId);
            recordContext.Value((int)record.OwnerType);
            recordContext.Value(record.OwnerId);
            recordContext.Value(record.RoutingProductId);
            recordContext.Value(record.BED);
            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Null();
        }
        #endregion
    }
}
