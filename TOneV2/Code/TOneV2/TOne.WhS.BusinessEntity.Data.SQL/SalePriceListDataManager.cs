using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SalePriceListDataManager : BaseSQLDataManager, ISalePriceListDataManager
    {
        private readonly string[] _columns =
        {
            "ID", "OwnerType", "OwnerID", "CurrencyID", "EffectiveOn", "PriceListType",
            "SourceID", "ProcessInstanceID", "FileID","UserID","Description"
        };
        private readonly string[] _newPricelistColumns =
        {
            "ID", "OwnerType", "OwnerID", "CurrencyID", "EffectiveOn", "PriceListType",
             "ProcessInstanceID", "FileID","UserID","Description"
        };
        #region ctor/Local Variables
        public SalePriceListDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public List<Entities.SalePriceList> GetPriceLists()
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePriceList_GetAll", SalePriceListMapper);
        }

        public bool ArGetSalePriceListsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SalePriceList", ref updateHandle);
        }

        public bool Update(SalePriceList salePriceList)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceList_Update", salePriceList.PriceListId, salePriceList.FileId, (int)salePriceList.PriceListType);
            return (recordsEffected > 0);
        }
        public bool SetCustomerPricelistsAsSent(IEnumerable<int> customerIds, int? priceListId)
        {
            string customerIdsStr = null;
            if (customerIds != null && customerIds.Any())
                customerIdsStr = string.Join(",", customerIds);
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceList_SetCustomerPricelistsAsSent", customerIdsStr, priceListId);
            return (recordsEffected > 0);
        }
        public bool Insert(SalePriceList salePriceList)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceList_Insert", salePriceList.PriceListId, (int)salePriceList.OwnerType, salePriceList.OwnerId, (int)salePriceList.PriceListType, salePriceList.CurrencyId,
                salePriceList.EffectiveOn, salePriceList.ProcessInstanceId, salePriceList.FileId, salePriceList.UserId);
            return (recordsEffected > 0);
        }
        public void SavePriceListsToDb(List<SalePriceList> salePriceLists)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (SalePriceList salePriceList in salePriceLists)
                WriteRecordToStream(salePriceList, dbApplyStream);
            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelist_New", _columns);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }
        public void SavePriceListsToDb(IEnumerable<NewPriceList> salePriceLists)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (NewPriceList salePriceList in salePriceLists)
                WriteRecordToStream(salePriceList, dbApplyStream);
            Object preparedSalePriceLists = FinishDBApplyStream(dbApplyStream, "TOneWhS_BE.SalePricelist_New", _newPricelistColumns);
            ApplySalePriceListsToDB(preparedSalePriceLists);
        }
        #endregion

        #region Private Methods

        private void WriteRecordToStream(SalePriceList record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}",
                    record.PriceListId,
                    (int)record.OwnerType,
                    record.OwnerId,
                    record.CurrencyId,
                    GetDateTimeForBCP(record.EffectiveOn),
                    (int?)record.PriceListType,
                    record.SourceId,
                    record.ProcessInstanceId,
                    record.FileId,
                    record.UserId,
                    record.Description);
        }
        private void WriteRecordToStream(NewPriceList record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            if (streamForBulkInsert != null)
                streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}",
                    record.PriceListId,
                    (int)record.OwnerType,
                    record.OwnerId,
                    record.CurrencyId,
                    GetDateTimeForBCP(record.EffectiveOn),
                    (int?)record.PriceListType,
                    record.ProcessInstanceId,
                    record.FileId,
                    record.UserId,
                    record.Description);
        }
        private void ApplySalePriceListsToDB(object preparedSalePriceLists)
        {
            InsertBulkToTable(preparedSalePriceLists as BaseBulkInsertInfo);
        }
        private object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        private object FinishDBApplyStream(object dbApplyStream, string tableName, string[] columnNames)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = tableName,
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columnNames
            };

        }
        #endregion

        #region Mappers
        SalePriceList SalePriceListMapper(IDataReader reader)
        {
            SalePriceList salePriceList = new SalePriceList
            {
                OwnerId = (int)reader["OwnerID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                OwnerType = (Entities.SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn"),
                PriceListType = (SalePriceListType?)GetReaderValue<byte?>(reader, "PriceListType"),
                ProcessInstanceId = GetReaderValue<long>(reader, "ProcessInstanceID"),
                FileId = GetReaderValue<long>(reader, "FileID"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                IsSent = GetReaderValue<bool>(reader, "IsSent"),
                SourceId = GetReaderValue<string>(reader, "SourceID"),
                UserId = GetReaderValue<int>(reader, "UserID"),
                Description = GetReaderValue<string>(reader,"Description"),
                PricelistStateBackupId = GetReaderValue<long?>(reader, "PricelistStateBackupID"),
                PricelistSource = (SalePricelistSource?)GetReaderValue<byte?>(reader, "PricelistSource"),
            };

            return salePriceList;
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
                                                   ([ID]
                                                   ,[OwnerType]
                                                   ,[OwnerID]
                                                   ,[CurrencyID]
                                                   ,[EffectiveOn]
                                                   ,[PriceListType]
                                                   ,[SourceID]
                                                   ,[ProcessInstanceID]
                                                   ,[FileID]
                                                   ,[IsSent]
                                                   ,[UserID]
                                                   ,[CreatedTime]
                                                   ,[Description]
                                                   ,[PricelistStateBackupID]
                                                   ,[PricelistSource]
                                                   ,[StateBackupID])
                                            SELECT 
                                                     sp.[ID]
		                                            ,sp.[OwnerType]
		                                            ,sp.[OwnerID]
		                                            ,sp.[CurrencyID]
		                                            ,sp.[EffectiveOn]
		                                            ,sp.[PriceListType]
		                                            ,sp.[SourceID]
		                                            ,sp.[ProcessInstanceID]
		                                            ,sp.[FileID]
                                                    ,sp.IsSent
                                                    ,sp.[UserID]
		                                            ,sp.[CreatedTime]
                                                    ,sp.[Description]
                                                    ,sp.[PricelistStateBackupID]
                                                    ,sp.[PricelistSource]
                                                    ,{1} AS StateBackupID  
                                            FROM [TOneWhS_BE].[SalePriceList] sp WITH (NOLOCK) 
                                            Inner Join [TOneWhS_BE].CarrierAccount ca WITH (NOLOCK) on sp.OwnerID = ca.ID
											Inner Join [TOneWhS_BE].SellingNumberPlan np on ca.SellingNumberPlanID=np.ID
                                            Where ca.SellingNumberPlanID = {2} and sp.OwnerType=1  
                                            Union 
                                            SELECT 
                                                     sp.[ID]
		                                            ,sp.[OwnerType]
		                                            ,sp.[OwnerID]
		                                            ,sp.[CurrencyID]
		                                            ,sp.[EffectiveOn]
                                                    ,sp.[PriceListType]
		                                            ,sp.[SourceID]
		                                            ,sp.[ProcessInstanceID]
		                                            ,sp.[FileID]
                                                    ,sp.IsSent
                                                    ,sp.[UserID] 
		                                            ,sp.[CreatedTime]
                                                    ,sp.[Description]
                                                    ,sp.[PricelistStateBackupID]
                                                    ,sp.[PricelistSource]
                                                    ,{1} AS StateBackupID 
                                           FROM [TOneWhS_BE].[SalePriceList] sp WITH (NOLOCK) 
                                           Inner Join [TOneWhS_BE].SellingProduct sellp WITH (NOLOCK) on sp.OwnerID = sellp.ID
										   Inner Join [TOneWhS_BE].SellingNumberPlan np on sellp.SellingNumberPlanID=np.ID
                                           Where sellp.SellingNumberPlanID = {2} and sp.OwnerType=0 ", backupDatabase,
                stateBackupId, sellingNumberPlanId);
        }


        public string BackupAllDataByOwner(long stateBackupId, string backupDatabase, IEnumerable<int> ownerIds, int ownerType)
        {
            string ownerIdsString = null;
            if (ownerIds != null && ownerIds.Any())
                ownerIdsString = string.Join<int>(",", ownerIds);

            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
                                                   ([ID]
                                                   ,[OwnerType]
                                                   ,[OwnerID]
                                                   ,[CurrencyID]
                                                   ,[EffectiveOn]
                                                   ,[PriceListType]
                                                   ,[SourceID]
                                                   ,[ProcessInstanceID]
                                                   ,[FileID]
                                                   ,[IsSent]
                                                   ,[CreatedTime]
                                                   ,[UserID]
                                                   ,[Description]
                                                   ,[PricelistStateBackupID]
                                                   ,[PricelistSource]
                                                   ,[StateBackupID])
                                            SELECT  [ID]
                                                   ,[OwnerType]
                                                   ,[OwnerID]
                                                   ,[CurrencyID]
                                                   ,[EffectiveOn]
                                                   ,[PriceListType]
                                                   ,[SourceID]
                                                   ,[ProcessInstanceID]
                                                   ,[FileID]
                                                   ,[IsSent]
                                                   ,[CreatedTime]
                                                   ,[UserID]
                                                   ,[Description]
                                                   ,[PricelistStateBackupID]
                                                   ,[PricelistSource]
                                                   ,{1} AS StateBackupID 
                                            FROM [TOneWhS_BE].[SalePriceList]
                                            WITH (NOLOCK) Where OwnerId IN({2}) and OwnerType = {3}", backupDatabase, stateBackupId, ownerIdsString, ownerType);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SalePriceList] 
                                                    ([ID] 
                                                    ,[OwnerType] 
                                                    ,[OwnerID] 
                                                    ,[CurrencyID] 
                                                    ,[EffectiveOn] 
                                                    ,[PriceListType]
                                                    ,[SourceID] 
                                                    ,[ProcessInstanceID]
                                                    ,[FileID]
                                                    ,[CreatedTime]
                                                    ,[IsSent]
                                                    ,[UserID]
                                                    ,[Description]
                                                    ,[PricelistStateBackupID]
                                                    ,[PricelistSource])
                                                                                               
                                            SELECT   [ID] 
                                                    ,[OwnerType] 
                                                    ,[OwnerID] 
                                                    ,[CurrencyID] 
                                                    ,[EffectiveOn] 
                                                    ,[PriceListType]
                                                    ,[SourceID] 
                                                    ,[ProcessInstanceID]
                                                    ,[FileID]
                                                    ,[CreatedTime]
                                                    ,[IsSent]
                                                    ,[UserID]
                                                    ,[Description]
                                                    ,[PricelistStateBackupID]
                                                    ,[PricelistSource]
                                            FROM [{0}].[TOneWhS_BE_Bkup].[SalePriceList]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }


        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"Delete FROM [TOneWhS_BE].[SalePriceList] where ID in (SELECT sp.[ID] FROM [TOneWhS_BE].[SalePriceList]
                                            sp WITH (NOLOCK) Inner Join [TOneWhS_BE].CarrierAccount ca WITH (NOLOCK) on sp.OwnerID = ca.ID
											Inner Join [TOneWhS_BE].SellingNumberPlan np on ca.SellingNumberPlanID=np.ID
                                            Where ca.SellingNumberPlanID = {0} and sp.OwnerType=1  Union SELECT sp.[ID] FROM [TOneWhS_BE].[SalePriceList]
                                            sp WITH (NOLOCK) Inner Join [TOneWhS_BE].SellingProduct sellp WITH (NOLOCK) on sp.OwnerID = sellp.ID
											Inner Join [TOneWhS_BE].SellingNumberPlan np on sellp.SellingNumberPlanID=np.ID
                                            Where sellp.SellingNumberPlanID = {0} and sp.OwnerType=0)", sellingNumberPlanId);
        }

        public string GetDeleteCommandsByOwner(IEnumerable<int> ownerIds, int ownerType)
        {
            string ownerIdsString = null;
            if (ownerIds != null && ownerIds.Any())
                ownerIdsString = string.Join(",", ownerIds);
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SalePriceList]
                                           Where OwnerId IN ( {0} ) and OwnerType = {1}", ownerIdsString, ownerType);
        }


        #endregion




    }
}
