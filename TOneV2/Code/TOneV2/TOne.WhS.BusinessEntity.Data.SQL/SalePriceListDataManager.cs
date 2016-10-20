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

        public string BackupAllData(int stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [{0}].[dbo].[SalePriceList] WITH (TABLOCK)
                                            SELECT {1} AS StateBackupID, [ID] ,[OwnerType] ,[OwnerID] ,[CurrencyID] ,[EffectiveOn], [SourceID] FROM [TOneWhS_BE].[SalePriceList]
                                            WITH (NOLOCK) Where OwnerType = 1", backupDatabase, stateBackupId);
        }

        public string BackupAllDataByCustomerId(int stateBackupId, string backupDatabase, int customerId)
        {
            return String.Format(@"INSERT INTO [{0}].[dbo].[SalePriceList] WITH (TABLOCK)
                                            SELECT {1} AS StateBackupID, [ID] ,[OwnerType] ,[OwnerID] ,[CurrencyID] ,[EffectiveOn], [SourceID] FROM [TOneWhS_BE].[SalePriceList]
                                            WITH (NOLOCK) Where OwnerType = 1 And OwnerId = {2}", backupDatabase, stateBackupId, customerId);
        }

        #endregion

    }
}
