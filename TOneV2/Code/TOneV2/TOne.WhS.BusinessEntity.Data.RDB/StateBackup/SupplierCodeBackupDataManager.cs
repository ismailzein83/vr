using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;


namespace TOne.WhS.BusinessEntity.Data.RDB.StateBackup
{
    public class SupplierCodeBackupDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "supcbkup";
        public static string TABLE_NAME = "TOneWhS_BE_Bkup_SupplierCode";
        const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static SupplierCodeBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_ZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CodeGroupID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SupplierCode",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion
    }
}
