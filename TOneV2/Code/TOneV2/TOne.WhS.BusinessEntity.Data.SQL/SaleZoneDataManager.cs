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

        public bool UpdateSaleZoneName(long zoneId, string zoneName, int sellingNumberPlanId)
        {
            int recordsEffected = ExecuteNonQuerySP("[TOneWhS_BE].[sp_SaleZone_UpdateName]", zoneId, zoneName, sellingNumberPlanId);
            return (recordsEffected > 0);
        }
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
        public IOrderedEnumerable<long> GetSaleZoneIds(DateTime? effectiveOn, bool isEffectiveInFuture)
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
            return saleZoneIds.OrderBy(itm => itm);
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
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"]

            };
            return saleZoneInfo;
        }
        #endregion


        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleZone] WITH (TABLOCK)
                                                       ( [ID]
                                                       , [SellingNumberPlanID]
                                                       , [CountryID]
                                                       , [Name]
                                                       , [BED]
                                                       , [EED]
                                                       , [SourceID]
                                                       , [StateBackupID]
                                                       , [ProcessInstanceID])
                                                 SELECT [ID]
                                                        , [SellingNumberPlanID]
                                                        , [CountryID]
                                                        , [Name]
                                                        , [BED]
                                                        , [EED]
                                                        , [SourceID]
                                                        , {1} AS StateBackupID
                                                        ,[ProcessInstanceID]
                                                FROM [TOneWhS_BE].[SaleZone]
                                                Where SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleZone] 
                                                ( [ID]
                                                , [SellingNumberPlanID]
                                                , [CountryID]
                                                , [Name]
                                                , [BED]
                                                , [EED]
                                                , [SourceID]
                                                , [ProcessInstanceID])
                                         SELECT   [ID]
                                                , [SellingNumberPlanID]
                                                , [CountryID]
                                                , [Name]
                                                , [BED]
                                                , [EED]
                                                , [SourceID] 
                                                , [ProcessInstanceID]
                                        FROM [{0}].[TOneWhS_BE_Bkup].[SaleZone]
                                        WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }


        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SaleZone] Where SellingNumberPlanID = {0}", sellingNumberPlanId);
        }

        #endregion
    }
}
