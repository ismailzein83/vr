﻿using System;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierZoneDataManager : ISupplierZoneDataManager
    {

        #region RDB

        static string TABLE_ALIAS = "spz";
        static string TABLE_NAME = "TOneWhS_BE_SupplierZone";

        internal const string COL_ID = "ID";
        internal const string COL_Name = "Name";
        internal const string COL_CountryID = "CountryID";
        internal const string COL_SupplierID = "SupplierID";

        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static SupplierZoneDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Name, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255}},
                {COL_SupplierID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierZone",
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

        #region ISupplierZoneDataManager Members

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);

            BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveDate);

            return queryContext.GetItems(SupplierZoneMapper);
        }

        public List<SupplierZone> GetSupplierZones()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SupplierZoneMapper);
        }

        public bool AreSupplierZonesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_SupplierID).Value(supplierId);

            var orCondition = whereQuery.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(COL_EED);
            orCondition.GreaterThanCondition(COL_EED).Value(minimumDate);

            return queryContext.GetItems(SupplierZoneMapper);
        }

        public List<SupplierZone> GetEffectiveSupplierZones(int supplierId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();

            if (effectiveOn.HasValue)
            {
                if (isEffectiveInFuture)
                {
                    BEDataUtility.SetEffectiveAfterDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                }
                else
                {
                    BEDataUtility.SetFutureDateCondition(whereQuery, TABLE_ALIAS, COL_BED, COL_EED, effectiveOn.Value);
                }
            }
            else
                whereQuery.FalseCondition();

            return queryContext.GetItems(SupplierZoneMapper);
        }
        #endregion

        #region Public Methods

        public void JoinSupplierZone(RDBJoinContext joinContext, string zoneTableAlias, string originalTableAlias, string originalTableZoneIdCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, zoneTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableZoneIdCol, zoneTableAlias, COL_ID);
        }

        #endregion

        #region Mappers
        SupplierZone SupplierZoneMapper(IRDBDataReader reader)
        {
            return new SupplierZone
            {
                SupplierId = reader.GetInt(COL_SupplierID),
                CountryId = reader.GetInt(COL_CountryID),
                SupplierZoneId = reader.GetInt(COL_ID),
                Name = reader.GetString(COL_Name),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                SourceId = reader.GetString(COL_SourceID)
            };
        }

        #endregion

    }
}