﻿using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierPriceListDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISupplierPriceListDataManager
    {

        #region ctor/Local Variables
        public SupplierPriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public List<SupplierPriceList> GetPriceLists()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierPriceList_GetAll", SupplierPriceListMapper);
        }
        public bool ArGetPriceListsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierPriceList", ref updateHandle);
        }


        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        SupplierPriceList SupplierPriceListMapper(IDataReader reader)
        {
            SupplierPriceList supplierPriceList = new SupplierPriceList
            {
                SupplierId = (int)reader["SupplierID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                FileId = GetReaderValue<long?>(reader, "FileID"),
                CreateTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                ProcessInstanceId = GetReaderValue<long?>(reader, "ProcessInstanceID"),
                SPLStateBackupId = GetReaderValue<long?>(reader, "SPLStateBackupID"),
                UserId = GetReaderValue<int>(reader, "UserID"),
                PricelistType = (SupplierPricelistType?)GetReaderValue<int>(reader, "PricelistType"),
                EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn")
            };
            return supplierPriceList;
        }

        #endregion


        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierPriceList] WITH (TABLOCK)
                                            ([ID]
                                           ,[SupplierID]
                                           ,[CurrencyID]
                                           ,[FileID]
                                           ,[EffectiveOn]
                                           ,[PricelistType]
                                           ,[CreatedTime]
                                           ,[SourceID]
                                           ,[ProcessInstanceID]
                                           ,[SPLStateBackupID]
                                           ,[UserID]
                                           ,[StateBackupID]
                                           ,[LastModifiedTime]
                                            )
                                            SELECT [ID] ,[SupplierID], [CurrencyID], [FileID], [EffectiveOn],[PricelistType], [CreatedTime], [SourceID],[ProcessInstanceID],[SPLStateBackupID],[UserID], {1} AS StateBackupID ,[LastModifiedTime]
                                  FROM [TOneWhS_BE].[SupplierPriceList]  WITH (NOLOCK) 
                                  Where SupplierID = {2}", backupDatabase, stateBackupId, supplierId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierPriceList] ([ID], [SupplierID], [CurrencyID], [FileID], [EffectiveOn],[PricelistType], [CreatedTime], [SourceID],[ProcessInstanceID],[SPLStateBackupID],[UserID],[LastModifiedTime])
                                            SELECT [ID], [SupplierID], [CurrencyID], [FileID], [EffectiveOn],[PricelistType], [CreatedTime], [SourceID],[ProcessInstanceID],[SPLStateBackupID],[UserID],[LastModifiedTime] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierPriceList]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySupplierId(int supplierId)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SupplierPriceList] Where SupplierID = {0}", supplierId);
        }

        #endregion
    }
}
