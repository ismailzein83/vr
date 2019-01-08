using System;
using System.Linq;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleZoneDataManager : ISaleZoneDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "TOneWhS_BE";
        static string TABLE_NAME = "TOneWhS_BE_SaleZone";
        const string COL_ID = "ID";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_CountryID = "CountryID";
        const string COL_Name = "Name";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";
        static SaleZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_SellingNumberPlanID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };

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
            where.ConditionIfColumnNotNull(COL_SellingNumberPlanID).EqualsCondition(COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            if (!string.IsNullOrEmpty(filter))
                where.ContainsCondition(TABLE_ALIAS, COL_Name, filter);

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SaleZoneInfoMapper);
        }

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
            var where = selectQuery.Where();
            AddIsEffectiveQuery(where, effectiveOn, isEffectiveInFuture);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(reader => reader.GetLong(COL_ID)).OrderBy(itm => itm);
        }

        public bool UpdateSaleZoneName(long zoneId, string zoneName, int sellingNumberPlanId)
        {
            throw new NotImplementedException();

            //Todo
            // [SalePricelistRateChange] , [SalePricelistCodeChange] , [SalePricelistRPChange] convert it first to RDB
        }
        #endregion

        #region Mappers
        SaleZone SaleZoneMapper(IRDBDataReader reader)
        {
            return new SaleZone
            {
                SaleZoneId = reader.GetLong("ID"),
                SellingNumberPlanId = reader.GetInt("SellingNumberPlanID"),
                CountryId = reader.GetInt("CountryID"),
                Name = reader.GetString("Name"),
                BED = reader.GetDateTime("BED"),
                EED = reader.GetNullableDateTime("EED"),
                SourceId = reader.GetString("SourceID")
            };
        }
        SaleZoneInfo SaleZoneInfoMapper(IRDBDataReader reader)
        {
            return new SaleZoneInfo
            {
                SaleZoneId = reader.GetLong("ID"),
                Name = reader.GetString("Name"),
                SellingNumberPlanId = reader.GetInt("SellingNumberPlanID")
            };
        }
        #endregion

        #region Private Methods

        private void AddIsEffectiveQuery(RDBConditionContext conditionContext, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            if (isEffectiveInFuture.HasValue)
            {
                if (isEffectiveInFuture.Value)
                {
                    conditionContext.ConditionIfColumnNotNull(TABLE_ALIAS, COL_EED).GreaterOrEqualCondition(TABLE_ALIAS, COL_BED).DateNow();
                }
                else if (effectiveDate.HasValue)
                {
                    conditionContext.LessOrEqualCondition(TABLE_ALIAS, COL_BED).Value(effectiveDate.Value);
                    var orCondition = conditionContext.ChildConditionGroup(RDBConditionGroupOperator.OR);
                    orCondition.ConditionIfColumnNotNull(TABLE_ALIAS, COL_EED).GreaterOrEqualCondition(TABLE_ALIAS, COL_EED).Value(effectiveDate.Value);
                }
            }
        }
        #endregion

        #region Public Methods

        public void JoinSaleZone(RDBJoinContext joinContext, string zoneTableAlias, string zoneTableCol, string originalTableAlias, string originalTableCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, zoneTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableCol, zoneTableAlias, zoneTableCol);
        }
        #endregion
    }
}
