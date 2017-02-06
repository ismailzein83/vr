using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SalePriceListDataManager : BaseSQLDataManager, ISalePriceListDataManager
    {

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

        public bool Insert(SalePriceList salePriceList)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceList_Insert", salePriceList.PriceListId, (int)salePriceList.OwnerType, salePriceList.OwnerId, (int)salePriceList.PriceListType, salePriceList.CurrencyId, 
                salePriceList.EffectiveOn, salePriceList.ProcessInstanceId, salePriceList.FileId);
            return (recordsEffected > 0);
        }

        #endregion

        #region Private Methods
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
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };

            return salePriceList;
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
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
		                                            ,sp.[CreatedTime]
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
		                                            ,sp.[CreatedTime]
                                                    ,{1} AS StateBackupID 
                                           FROM [TOneWhS_BE].[SalePriceList] sp WITH (NOLOCK) 
                                           Inner Join [TOneWhS_BE].SellingProduct sellp WITH (NOLOCK) on sp.OwnerID = sellp.ID
										   Inner Join [TOneWhS_BE].SellingNumberPlan np on sellp.SellingNumberPlanID=np.ID
                                           Where sellp.SellingNumberPlanID = {2} and sp.OwnerType=0 ", backupDatabase,
                stateBackupId, sellingNumberPlanId);
        }


        public string BackupAllDataByOwner(long stateBackupId, string backupDatabase, int ownerId, int ownerType)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
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
                                                    ,{1} AS StateBackupID FROM [TOneWhS_BE].[SalePriceList]
                                            WITH (NOLOCK) Where OwnerId = {2} and OwnerType = {3}", backupDatabase, stateBackupId, ownerId, ownerType);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SalePriceList] ( [ID] ,[OwnerType] ,[OwnerID] ,[CurrencyID] ,[EffectiveOn], [SourceID])
                                            SELECT [ID] ,[OwnerType] ,[OwnerID] ,[CurrencyID] ,[EffectiveOn], [SourceID] FROM [{0}].[TOneWhS_BE_Bkup].[SalePriceList]
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

        public string GetDeleteCommandsByOwner(int ownerId, int ownerType)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SalePriceList]
                                           Where OwnerId = {0} and OwnerType = {1}", ownerId, ownerType);
        }


        #endregion

    }
}
