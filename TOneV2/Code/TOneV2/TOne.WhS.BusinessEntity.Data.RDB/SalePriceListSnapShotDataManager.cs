using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListSnapShotDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "splss";
        static string TABLE_NAME = "TOneWhS_BE_SalePriceListSnapShot";
        const string COL_PriceListID = "PriceListID";
        const string COL_SnapShotDetail = "SnapShotDetail";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        static SalePriceListSnapShotDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_PriceListID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_SnapShotDetail, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceListSnapShot",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion
        #region Public Methods

        public void Bulk(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var salePriceListCodeSnapshot in salePriceListSaleCodeSnapshots)
            {
                WriteRecordToStream(salePriceListCodeSnapshot, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }
        public SalePriceListSnapShot GetSalePriceListSnapShot(int priceListId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_PriceListID).Value(priceListId);

            return queryContext.GetItem(SalePricelistSnapShotMapper);
        }
        #endregion

        #region Bulk Methods
        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_PriceListID, COL_SnapShotDetail);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(SalePriceListSnapShot record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.PriceListId);

            if (record.SnapShotDetail != null)
                recordContext.Value(Serializer.Serialize(record.SnapShotDetail));
            else
                recordContext.Value(string.Empty);

        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        private void ApplyForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();

        }

        #endregion

        #region Mapper

        SalePriceListSnapShot SalePricelistSnapShotMapper(IRDBDataReader reader)
        {
            return new SalePriceListSnapShot
            {
                PriceListId = reader.GetInt(COL_PriceListID),
                SnapShotDetail = Vanrise.Common.Serializer.Deserialize<SnapShotDetail>(reader.GetString(COL_SnapShotDetail))
            };
        }

        #endregion

        #region Public Methods

        public bool AreSalePriceListCodeSnapShotUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion
    }
}
