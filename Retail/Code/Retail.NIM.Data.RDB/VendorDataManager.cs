using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.NIM.Data.RDB
{
    public class VendorDataManager
    {
        public static string TABLE_NAME = "NIM_Vendor";

        public const string COL_Id = "Id";
        public const string COL_Name = "Name";
        public const string COL_CreatedBy = "CreatedBy";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedBy = "LastModifiedBy";
        public const string COL_LastModifiedTime = "LastModifiedTime";

        static VendorDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "NIM",
                DBTableName = "Vendor",
                Columns = columns,
                IdColumnName = COL_Id,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        public void AddJoinVendorById(RDBJoinContext joinContext, RDBJoinType joinType, string vendorTableAlias, string originalTableAlias, string originalTableVendorCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, vendorTableAlias, COL_Id, originalTableAlias, originalTableVendorCol, withNoLock);
        }
    }
}