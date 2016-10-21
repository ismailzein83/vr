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
            };
            return salePriceList;
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
                                            SELECT sp.[ID], sp.[OwnerType], sp.[OwnerID], sp.[CurrencyID], sp.[EffectiveOn], sp.[SourceID], {1} AS StateBackupID  FROM [TOneWhS_BE].[SalePriceList]
                                            sp WITH (NOLOCK) Inner Join [TOneWhS_BE].SaleRate sr WITH (NOLOCK) on sp.ID = sr.PriceListID 
                                            Inner join [TOneWhS_BE].SaleZone sz WITH (NOLOCK) on sr.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }


        public string BackupAllDataByOwner(long stateBackupId, string backupDatabase, int ownerId, int ownerType)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SalePriceList] WITH (TABLOCK)
                                            SELECT [ID] ,[OwnerType] ,[OwnerID] ,[CurrencyID] ,[EffectiveOn], [SourceID], {1} AS StateBackupID FROM [TOneWhS_BE].[SalePriceList]
                                            WITH (NOLOCK) Where OwnerId = {2} and OwnerType = {3}", backupDatabase, stateBackupId, ownerId, ownerType);
        }

        #endregion

    }
}
