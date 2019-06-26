using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.NIM.Data.RDB
{
    public class OLTDataManager
    {
        public static string TABLE_NAME = "NIM_PSTN_OLT";

        public const string COL_Id = "Id";
        public const string COL_Name = "Name";
        public const string COL_Site = "Site";
        public const string COL_Region = "Region";
        public const string COL_City = "City";
        public const string COL_Town = "Town";
        public const string COL_CreatedBy = "CreatedBy";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedBy = "LastModifiedBy";
        public const string COL_LastModifiedTime = "LastModifiedTime";
        public const string COL_IMS = "IMS";

        static OLTDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Site, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Region, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_City, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Town, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_IMS, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "NIM_PSTN",
                DBTableName = "OLT",
                Columns = columns,
                IdColumnName = COL_Id,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        public void AddJoinOLTById(RDBJoinContext joinContext, RDBJoinType joinType, string oltTableAlias, string originalTableAlias, string originalTableOLTCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, oltTableAlias, COL_Id, originalTableAlias, originalTableOLTCol, withNoLock);
        }
    }
}