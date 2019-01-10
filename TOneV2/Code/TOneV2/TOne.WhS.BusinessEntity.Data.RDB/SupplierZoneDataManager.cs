using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierZoneDataManager
    {

        #region RDB

        static string TABLE_ALIAS = "spz";
        static string TABLE_NAME = "TOneWhS_BE_SupplierZone";

        internal const string COL_ID = "ID";
        internal const string COL_Name = "Name";
        internal const string COL_CountryID = "CountryID";

        const string COL_SupplierID = "SupplierID";
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

        #region Public Methods

        public void JoinSupplierZone(RDBJoinContext joinContext, string zoneTableAlias, string originalTableAlias, string originalTableZoneIdCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, zoneTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableZoneIdCol, zoneTableAlias, COL_ID);
        }

        #endregion
    }
}
