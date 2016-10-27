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
    public class SaleZoneDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleZoneDataManager
    {

        #region ctor/Local Variables
        public SaleZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public IEnumerable<SaleZone> GetAllSaleZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetAll", SaleZoneMapper);
        }
        public List<SaleZone> GetSaleZones(int sellingNumberPlanId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZone_GetByNumberPlan", SaleZoneMapper, sellingNumberPlanId);
        }
        public List<SaleZoneInfo> GetSaleZonesInfo(int sellingNumberPlanId, string filter)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleZoneInfo_GetFiltered", SaleZoneInfoMapper, sellingNumberPlanId, filter);
        }
        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("TOneWhS_BE.SaleZone", ref lastReceivedDataInfo);
        }
        public IEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<long> saleZoneIds = new List<long>();
            ExecuteReaderSP("[TOneWhS_BE].[sp_SaleZone_GetIds]", (reader) =>
            {
                while (reader.Read())
                {
                    long saleZoneId = GetReaderValue<Int64>(reader, "Id");
                    saleZoneIds.Add(saleZoneId);
                }
            }, effectiveOn, isEffectiveInFuture);
            return saleZoneIds;
        }

        #endregion

        #region Private Methods
        #endregion

        #region Mappers

        SaleZone SaleZoneMapper(IDataReader reader)
        {
            SaleZone saleZone = new SaleZone();

            saleZone.SaleZoneId = (long)reader["ID"];
            saleZone.SellingNumberPlanId = (int)reader["SellingNumberPlanID"];
            saleZone.CountryId = GetReaderValue<int>(reader, "CountryID");
            saleZone.Name = reader["Name"] as string;
            saleZone.BED = GetReaderValue<DateTime>(reader, "BED");
            saleZone.EED = GetReaderValue<DateTime?>(reader, "EED");
            saleZone.SourceId = reader["SourceID"] as string;
            return saleZone;
        }

        SaleZoneInfo SaleZoneInfoMapper(IDataReader reader)
        {
            SaleZoneInfo saleZoneInfo = new SaleZoneInfo
            {
                SaleZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                SellingNumberPlanId =  (int)reader["SellingNumberPlanID"]
                
            };
            return saleZoneInfo;
        }
        #endregion


        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleZone] WITH (TABLOCK)
                                            SELECT [ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID], {1} AS StateBackupID  FROM [TOneWhS_BE].[SaleZone]
                                            Where SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleZone] ([ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID])
                                            SELECT [ID], [SellingNumberPlanID], [CountryID], [Name], [BED], [EED], [SourceID] FROM [{0}].[TOneWhS_BE_Bkup].[SaleZone]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }


        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SaleZone] Where SellingNumberPlanID = {0}", sellingNumberPlanId);
        }


        public string GetDeleteCommandsByOwner(int ownerId, int ownerType)
        {
            return String.Format(@"DELETE sz FROM [TOneWhS_BE].[SaleRate] sr  Inner Join [TOneWhS_BE].[SalePriceList] pl  on sr.PriceListID = pl.ID
                                          Inner Join [TOneWhS_BE].[SaleZone] sz on sr.ZoneID = sz.ID 
                                          Where pl.OwnerId = {0} and pl.OwnerType = {1}", ownerId, ownerType);
        }

        #endregion
    }
}
