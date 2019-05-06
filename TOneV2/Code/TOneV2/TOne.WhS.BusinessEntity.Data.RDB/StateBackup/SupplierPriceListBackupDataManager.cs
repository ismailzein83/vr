﻿using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierPriceListBackupDataManager
    {
        #region RDB

        public static string TABLE_ALIAS = "supplbkup";
        public static string TABLE_NAME = "TOneWhS_BE_Bkup_SupplierPriceList";
        const string COL_ID = "ID";
        const string COL_SupplierID = "SupplierID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_FileID = "FileID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PricelistType = "PricelistType";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_SPLStateBackupID = "SPLStateBackupID";
        const string COL_UserID = "UserID";
        const string COL_LastModifiedTime = "LastModifiedTime";

        internal const string COL_StateBackupID = "StateBackupID";

        static SupplierPriceListBackupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SupplierID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_FileID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PricelistType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SPLStateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_StateBackupID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE_Bkup",
                DBTableName = "SupplierPriceList",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        #endregion
    }
}
