using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePriceList_New";
        static string TABLE_ALIAS = "splnew";

        const string COL_OwnerType = "OwnerType";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PriceListType = "PriceListType";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_FileID = "FileID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_IsSent = "IsSent";
        const string COL_UserID = "UserID";
        const string COL_Description = "Description";
        const string COL_PricelistStateBackupID = "PricelistStateBackupID";

        internal const string COL_ID = "ID";
        internal const string COL_OwnerID = "OwnerID";

        static SalePriceListNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PriceListType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_FileID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IsSent, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_PricelistStateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceList_New",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        
        #endregion

        #region Public Methods

        public List<SalePriceListNew> GeSalePriceListsNewByOwner(int ownerType, int priceListType, long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            whereContext.EqualsCondition(COL_OwnerType).Value(ownerType);
            whereContext.NotEqualsCondition(COL_PriceListType).Value(priceListType);

            return queryContext.GetItems(SalePriceListNewMapper);
        }
        public bool HasPriceListByProcessInstanceId(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        public void JoinSalePriceListNew(RDBJoinContext joinContext, string salePriceListTableAlias, string originalTableAlias, string originalTablePriceListIDCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, salePriceListTableAlias);
            joinStatement.WithNoLock();
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTablePriceListIDCol, salePriceListTableAlias, COL_ID);
        }
        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_CurrencyID, COL_EffectiveOn,
                COL_FileID, COL_PriceListType, COL_Description);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }

        public void BulkNewPriceLists(IEnumerable<NewPriceList> salePriceLists)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var salePriceList in salePriceLists)
            {
                WriteRecordToStream(salePriceList, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }
        #endregion

        #region Bulk 

        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ID, COL_OwnerType, COL_OwnerID, COL_CurrencyID, COL_EffectiveOn, COL_PriceListType, COL_ProcessInstanceID, COL_FileID, COL_UserID, COL_Description);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(NewPriceList record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            recordContext.Value(record.PriceListId);
            recordContext.Value((int)record.OwnerType);
            recordContext.Value(record.OwnerId);
            recordContext.Value(record.CurrencyId);
            recordContext.Value(record.EffectiveOn);

            if (record.PriceListType.HasValue)
                recordContext.Value((int)record.PriceListType);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.ProcessInstanceId);
            recordContext.Value(record.FileId);
            recordContext.Value(record.UserId);
            recordContext.Value(record.Description);
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

        #region Mappers
        SalePriceListNew SalePriceListNewMapper(IRDBDataReader reader)
        {
            return new SalePriceListNew
            {
                PriceListId = reader.GetInt(COL_ID),
                OwnerId = reader.GetInt(COL_OwnerID),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                EffectiveOn = reader.GetDateTimeWithNullHandling(COL_EffectiveOn),
                PriceListType = (SalePriceListType?)reader.GetNullableInt(COL_PriceListType),
                SourceId = reader.GetString(COL_SourceID),
                ProcessInstanceId = reader.GetLongWithNullHandling(COL_ProcessInstanceID),
                FileId = reader.GetLongWithNullHandling(COL_FileID),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                UserId = reader.GetIntWithNullHandling(COL_UserID),
                Description = reader.GetString(COL_Description),
                PricelistStateBackupId = reader.GetNullableLong(COL_PricelistStateBackupID)
            };
        }

        #endregion

    }
}
