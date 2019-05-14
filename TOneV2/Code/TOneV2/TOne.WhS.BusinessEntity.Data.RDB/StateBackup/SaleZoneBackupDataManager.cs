using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleZoneBackupDataManager
    {
        #region RDB

        public static string TABLE_ALIAS = "szbkup";
        public static string TABLE_NAME = "TOneWhS_BE_Bkup_SaleZone";
        const string COL_ID = "ID";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_CountryID = "CountryID";
        const string COL_Name = "Name";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";
        static SaleZoneBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SellingNumberPlanID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SaleZone",
                Columns = columns,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion
    }
}
