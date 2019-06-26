using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Retail.NIM.Data.RDB
{
    public class IMSSlotDataManager
    {
        public static string TABLE_NAME = "NIM_PSTN_IMSSlot";

        public const string COL_Id = "Id";
        public const string COL_Name = "Name";
        public const string COL_Card = "Card";
        public const string COL_CreatedBy = "CreatedBy";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedBy = "LastModifiedBy";
        public const string COL_LastModifiedTime = "LastModifiedTime";

        static IMSSlotDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Id, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Card, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "NIM_PSTN",
                DBTableName = "IMSSlot",
                Columns = columns,
                IdColumnName = COL_Id,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        public void AddJoinIMSSlotByIMSCard(RDBJoinContext joinContext, RDBJoinType joinType, string imsSlotTableAlias, string originalTableAlias, string originalTableIMSCardCol, bool withNoLock)
        {
            joinContext.JoinOnEqualOtherTableColumn(joinType, TABLE_NAME, imsSlotTableAlias, COL_Card, originalTableAlias, originalTableIMSCardCol, withNoLock);
        }
    }
}