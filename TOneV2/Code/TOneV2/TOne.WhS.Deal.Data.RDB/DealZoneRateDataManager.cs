using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Data.RDB
{
    public class DealZoneRateDataManager : IDealZoneRateDataManager
    {
        static string TABLE_NAME = "TOneWhS_Deal_DealZoneRate";
        static string TABLE_ALIAS = "dzr";
        const string COL_ID = "ID";
        const string COL_DealId = "DealId";
        const string COL_ZoneGroupNb = "ZoneGroupNb";
        const string COL_IsSale = "IsSale";
        const string COL_TierNb = "TierNb";
        const string COL_ZoneId = "ZoneId";
        const string COL_Rate = "Rate";
        const string COL_CurrencyId = "CurrencyId";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";

        //readonly string[] columns = { COL_ID, COL_DealId, COL_ZoneGroupNb, COL_IsSale, COL_TierNb, COL_ZoneId, COL_Rate, COL_CurrencyId, COL_BED, COL_EED };
        static DealZoneRateDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_DealId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneGroupNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsSale, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_TierNb, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Rate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_CurrencyId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Deal",
                DBTableName = "DealZoneRate",
                Columns = columns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Deal", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        public IEnumerable<DealZoneRate> GetDealZoneRatesByDealIds(bool isSale, IEnumerable<int> dealIds)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dealIdsTempTable = CreateDealIdsTempTable(queryContext, dealIds);

            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var joinContext = selectQuery.Join();
            var joinCondition = joinContext.Join(dealIdsTempTable, "aux").On();
            joinCondition.EqualsCondition("aux", COL_DealId, TABLE_ALIAS, COL_DealId);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);

            return queryContext.GetItems(DealZoneRateMapper);
        }

        public IEnumerable<DealZoneRate> GetDealZoneRatesByDate(bool isSale, DateTime fromDate, DateTime toDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_IsSale).Value(isSale);
            where.LessThanCondition(COL_BED).Value(toDate);

            var orCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);

            var andCondition = orCondition.ChildConditionGroup(RDBConditionGroupOperator.AND);
            andCondition.NotEqualsCondition(COL_EED).Column(COL_BED);
            andCondition.GreaterThanCondition(COL_EED).Value(fromDate);

            return queryContext.GetItems(DealZoneRateMapper);
        }

        public bool AreDealZoneRateUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #region TempTableFunctions
        public void InitializeDealZoneRateInsert(IEnumerable<int> dealIdsToKeep)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var tempTableName = "DealZoneRate_Temp";
            var tempTableSchema = "TOneWhS_Deal";

            var dropQuery = queryContext.AddDropTableQuery();
            dropQuery.TableName(tempTableName);

            var tempTableColumns = new Dictionary<string, RDBTableColumnDefinition>();
            tempTableColumns.Add(COL_ID, new RDBTableColumnDefinition { DBColumnName = "ID", DataType = RDBDataType.BigInt });
            tempTableColumns.Add(COL_DealId, new RDBTableColumnDefinition { DBColumnName = "DealId", DataType = RDBDataType.Int });
            tempTableColumns.Add(COL_ZoneGroupNb, new RDBTableColumnDefinition { DBColumnName = "ZoneGroupNb", DataType = RDBDataType.Int });
            tempTableColumns.Add(COL_IsSale, new RDBTableColumnDefinition { DBColumnName = "IsSale", DataType = RDBDataType.Boolean });
            tempTableColumns.Add(COL_TierNb, new RDBTableColumnDefinition { DBColumnName = "TierNb", DataType = RDBDataType.Int });
            tempTableColumns.Add(COL_ZoneId, new RDBTableColumnDefinition { DBColumnName = "ZoneId", DataType = RDBDataType.BigInt });
            tempTableColumns.Add(COL_Rate, new RDBTableColumnDefinition { DBColumnName = "Rate", DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            tempTableColumns.Add(COL_CurrencyId, new RDBTableColumnDefinition { DBColumnName = "CurrencyId", DataType = RDBDataType.Int });
            tempTableColumns.Add(COL_BED, new RDBTableColumnDefinition { DBColumnName = "BED", DataType = RDBDataType.DateTime });
            tempTableColumns.Add(COL_EED, new RDBTableColumnDefinition { DBColumnName = "EED", DataType = RDBDataType.DateTime });
            tempTableColumns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DBColumnName = "LastModifiedTime", DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(tempTableName, new RDBTableDefinition
            {
                DBSchemaName = tempTableSchema,
                DBTableName = tempTableName,
                Columns = tempTableColumns,
                IdColumnName = COL_ID,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });

            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(tempTableSchema, tempTableName);
            //createTableQuery.DBTableName(tempTableName);
            createTableQuery.AddColumn(COL_ID, COL_ID, RDBDataType.BigInt, null, null, true, true, true);
            createTableQuery.AddColumn(COL_DealId, COL_DealId, RDBDataType.Int, null, null, true, false, false);
            createTableQuery.AddColumn(COL_ZoneGroupNb, COL_ZoneGroupNb, RDBDataType.Int, null, null, true, false, false);
            createTableQuery.AddColumn(COL_IsSale, COL_IsSale, RDBDataType.Boolean, null, null, true, false, false);
            createTableQuery.AddColumn(COL_TierNb, COL_TierNb, RDBDataType.Int, null, null, true, false, false);
            createTableQuery.AddColumn(COL_ZoneId, COL_ZoneId, RDBDataType.BigInt, null, null, true, false, false);
            createTableQuery.AddColumn(COL_Rate, COL_Rate, RDBDataType.Decimal, 20, 8, true, false, false);
            createTableQuery.AddColumn(COL_CurrencyId, COL_CurrencyId, RDBDataType.Int, null, null, true, false, false);
            createTableQuery.AddColumn(COL_BED, COL_BED, RDBDataType.DateTime, null, null, true, false, false);
            createTableQuery.AddColumn(COL_EED, RDBDataType.DateTime);
            createTableQuery.AddColumn(COL_LastModifiedTime, RDBDataType.DateTime);

            var uniqueId = new Guid("10B149EE-7A77-4E26-85AD-FD0D92A90951");

            //var createPrimaryIndex = queryContext.AddCreateIndexQuery();
            //createPrimaryIndex.DBTableName(tempTableSchema, tempTableName);
            ////createPrimaryIndex.DBTableName(tempTableName);
            //createPrimaryIndex.IndexName($"[PK_DealZoneRate_'{uniqueId}']");
            //createPrimaryIndex.IndexType(RDBCreateIndexType.Clustered);
            //createPrimaryIndex.AddColumn(COL_ID);

            var createIndexQuery1 = queryContext.AddCreateIndexQuery();
            createIndexQuery1.DBTableName(tempTableSchema, tempTableName);
            //createIndexQuery1.DBTableName(tempTableName);
            createIndexQuery1.IndexName($"[IX_DealZoneRate_BED_'{uniqueId}']");
            createIndexQuery1.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery1.AddColumn(COL_BED);

            var createIndexQuery2 = queryContext.AddCreateIndexQuery();
            createIndexQuery2.DBTableName(tempTableSchema, tempTableName);
            //createIndexQuery2.DBTableName(tempTableName);
            createIndexQuery2.IndexName($"[IX_DealZoneRate_DealId_'{uniqueId}']");
            createIndexQuery2.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery2.AddColumn(COL_DealId);

            var createIndexQuery3 = queryContext.AddCreateIndexQuery();
            createIndexQuery3.DBTableName(tempTableSchema, tempTableName);
            //createIndexQuery3.DBTableName(tempTableName);
            createIndexQuery3.IndexName($"[IX_DealZoneRate_EED_'{uniqueId}']");
            createIndexQuery3.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery3.AddColumn(COL_EED);

            var createIndexQuery4 = queryContext.AddCreateIndexQuery();
            createIndexQuery4.DBTableName(tempTableSchema, tempTableName);
            //createIndexQuery4.DBTableName(tempTableName);
            createIndexQuery4.IndexName($"[IX_DealZoneRate_IsSale_'{uniqueId}']");
            createIndexQuery4.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery4.AddColumn(COL_IsSale);

            queryContext.ExecuteNonQuery();
        }

        public void FinalizeDealZoneRateInsert()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var swapTablesQuery = queryContext.AddSwapTablesQuery();
            swapTablesQuery.TableNames(TABLE_NAME, "DealZoneRate_Temp", false);

            queryContext.ExecuteNonQuery();
        }
        #endregion


        #region BCP
        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_DealId, COL_ZoneGroupNb, COL_IsSale, COL_TierNb, COL_ZoneId, COL_Rate, COL_CurrencyId, COL_BED, COL_EED);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(DealZoneRate record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.DealId);
            recordContext.Value(record.ZoneGroupNb);
            if (record.IsSale)
                recordContext.Value(1);
            else
                recordContext.Value(0);
            recordContext.Value(record.TierNb);
            recordContext.Value(record.ZoneId);
            recordContext.Value(record.Rate);
            recordContext.Value(record.CurrencyId);
            recordContext.Value(record.BED);

            if (record.EED.HasValue)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Value(string.Empty);
        }

        public void ApplyNewDealZoneRatesToDB(object preparedRates)
        {
            preparedRates.CastWithValidate<RDBBulkInsertQueryContext>("preparedRates").Apply();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }
        #endregion

        private RDBTempTableQuery CreateDealIdsTempTable(RDBQueryContext queryContext, IEnumerable<int> dealIds)
        {
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumn(COL_DealId, RDBDataType.Int);

            if (dealIds != null)
            {
                var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
                insertMultipleRowsQuery.IntoTable(tempTableQuery);

                foreach (var dealId in dealIds)
                {
                    var rowContext = insertMultipleRowsQuery.AddRow();
                    rowContext.Column(COL_DealId).Value(dealId);
                }
            }
            return tempTableQuery;
        }

        private DealZoneRate DealZoneRateMapper(IRDBDataReader reader)
        {
            return new DealZoneRate
            {
                DealZoneRateId = reader.GetLong(COL_ID),
                DealId = reader.GetInt(COL_DealId),
                ZoneGroupNb = reader.GetInt(COL_ZoneGroupNb),
                IsSale = reader.GetBoolean(COL_IsSale),
                TierNb = reader.GetInt(COL_TierNb),
                ZoneId = reader.GetLong(COL_ZoneId),
                Rate = reader.GetDecimal(COL_Rate),
                CurrencyId = reader.GetInt(COL_CurrencyId),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
        }


    }
}
