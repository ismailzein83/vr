using System;
using System.Linq;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleZoneDataManager : ISaleZoneDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "sz";
        static string TABLE_NAME = "TOneWhS_BE_SaleZone";
        public const string COL_ID = "ID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";

        internal const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        internal const string COL_CountryID = "CountryID";
        internal const string COL_Name = "Name";

        static SaleZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SellingNumberPlanID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleZone",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ISaleZoneDataManager Members

        public bool AreZonesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public IEnumerable<SaleZone> GetAllSaleZones()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SaleZoneMapper);
        }

        public IOrderedEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_ID);

            var whereContext = selectQuery.Where();

            BEDataUtility.SetDateCondition(whereContext, TABLE_ALIAS, COL_BED, COL_EED, isEffectiveInFuture, effectiveOn);

            return queryContext.GetItems(reader => reader.GetLong(COL_ID)).OrderBy(itm => itm);
        }

        public bool UpdateSaleZoneName(long zoneId, string zoneName, int sellingNumberPlanId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_Name).Value(zoneName);
            updateQuery.Where().EqualsCondition(COL_ID).Value(zoneId);

            var salePriceListRateChangeDataManager = new SalePricelistRateChangeDataManager();
            salePriceListRateChangeDataManager.SetUpdateContext(queryContext, zoneName, zoneId);

            var salePriceListCodeChangeDataManager = new SalePricelistCodeChangeDataManager();
            salePriceListCodeChangeDataManager.SetUpdateContext(queryContext, zoneName, zoneId);

            var salePriceListRPChangeDataManager = new SalePricelistRPChangeDataManager();
            salePriceListRPChangeDataManager.SetUpdateContext(queryContext, zoneName, zoneId);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        #endregion

        #region Mappers

        SaleZone SaleZoneMapper(IRDBDataReader reader)
        {
            return new SaleZone
            {
                SaleZoneId = reader.GetLong(COL_ID),
                SellingNumberPlanId = reader.GetInt(COL_SellingNumberPlanID),
                CountryId = reader.GetInt(COL_CountryID),
                Name = reader.GetString(COL_Name),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                SourceId = reader.GetString(COL_SourceID)
            };
        }
        SaleZoneInfo SaleZoneInfoMapper(IRDBDataReader reader)
        {
            return new SaleZoneInfo
            {
                SaleZoneId = reader.GetLong(COL_ID),
                Name = reader.GetString(COL_Name),
                SellingNumberPlanId = reader.GetInt(COL_SellingNumberPlanID)
            };
        }

        #endregion


        #region Public Methods

        public void JoinSaleZone(RDBJoinContext joinContext, string zoneTableAlias, string originalTableAlias, string originalTableZoneIdCol, bool withNoLockJoin)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, zoneTableAlias);
            if (withNoLockJoin)
                joinStatement.WithNoLock();
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableZoneIdCol, zoneTableAlias, COL_ID);
        }

        public void BuildInsertQuery(RDBInsertQuery insertQuery, long processInstanceID)
        {
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ProcessInstanceID).Value(processInstanceID);
        }

        public void BuildUpdateQuery(RDBUpdateQuery updateQuery, long processInstanceID, string joinTableAlias, string columnName)
        {
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column(joinTableAlias, COL_EED);
            updateQuery.Where().EqualsCondition(joinTableAlias, columnName).Value(processInstanceID);
        }

        #endregion

        #region Not Used Functions

        public List<SaleZone> GetSaleZones(int sellingNumberPlanId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.NotNullCondition(COL_SellingNumberPlanID);
            where.EqualsCondition(COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            return queryContext.GetItems(SaleZoneMapper);
        }

        public List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            var where = selectQuery.Where();
            where.NotNullCondition(COL_SellingNumberPlanID);
            where.EqualsCondition(COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            if (!string.IsNullOrEmpty(filter))
                where.ContainsCondition(TABLE_ALIAS, COL_Name, filter);

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SaleZoneInfoMapper);
        }

        #endregion
    }
}
