using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleEntityRoutingProductBackupDataManager
    {
        #region RDB

        public static string TABLE_ALIAS = "rpbkup";
        public static string TABLE_NAME = "TOneWhS_BE_Bkup_SaleEntityRoutingProduct";
        const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_ZoneID = "ZoneID";
        const string COL_RoutingProductID = "RoutingProductID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static SaleEntityRoutingProductBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OwnerType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OwnerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_RoutingProductID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SaleEntityRoutingProduct",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion
    }
}
