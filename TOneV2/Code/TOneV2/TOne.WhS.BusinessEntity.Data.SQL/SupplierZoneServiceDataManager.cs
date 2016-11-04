using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierZoneServiceDataManager : BaseSQLDataManager, ISupplierZoneServiceDataManager
    {
        #region ctor/Local Variables
        
        public SupplierZoneServiceDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion
       
        #region Public Methods
       
        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZonesServices_GetByDate", SupplierZoneServiceMapper, supplierId, minimumDate);
        }


        public SupplierDefaultService GetSupplierDefaultServiceBySupplier(int supplierId, DateTime effectiveOn)
        {
            return GetItemSP("TOneWhS_BE.sp_SupplierDefaultService_GetBySupplier", SupplierDefaultServiceMapper, supplierId, effectiveOn);
        }

        public bool Update(long supplierZoneServiceId, DateTime effectiveDate)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierZoneService_Update", supplierZoneServiceId, effectiveDate);
            return (recordsEffected > 0);
        }
        public bool Insert(SupplierDefaultService supplierZoneService)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SupplierZoneService_Insert", supplierZoneService.SupplierZoneServiceId, supplierZoneService.SupplierId, Vanrise.Common.Serializer.Serialize(supplierZoneService.ReceivedServices, false), Vanrise.Common.Serializer.Serialize(supplierZoneService.EffectiveServices, false), supplierZoneService.BED);
            return (recordsEffected > 0);
        }

        public bool AreSupplierZoneServicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierZoneService", ref updateHandle);
        }

        public IEnumerable<SupplierZoneService> GetEffectiveSupplierZoneServices(int supplierId, DateTime from, DateTime to)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZonesService_GetBySupplier", SupplierZoneServiceMapper, supplierId, from, to);
        }

        public IEnumerable<SupplierDefaultService> GetEffectiveSupplierDefaultServices(DateTime from, DateTime to)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierDefaultService_GetEffectiveDefaultServices", SupplierDefaultServiceMapper, from, to);
        }

        public List<SupplierDefaultService> GetEffectiveSupplierDefaultServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            //TODO: MJA the same but get all records with Zone Id NOT equal to null
            DataTable supplierZoneServicesOwners = BuildRoutingSupplierInfoTable(supplierInfos);
            return GetItemsSPCmd("TOneWhS_BE.sp_SupplierDefaultService_GetEffectiveDefaultServicesBySuppliers", SupplierDefaultServiceMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SupplierZoneServicesOwners", SqlDbType.Structured);
                dtPrm.Value = supplierZoneServicesOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        public List<SupplierZoneService> GetEffectiveSupplierZoneServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            //TODO: MJA the same but get all records with Zone Id NOT equal to null
            DataTable supplierZoneServicesOwners = BuildRoutingSupplierInfoTable(supplierInfos);
            return GetItemsSPCmd("TOneWhS_BE.sp_SupplierZoneService_GetEffectiveZoneServicesBySuppliers", SupplierZoneServiceMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SupplierZoneServicesOwners", SqlDbType.Structured);
                dtPrm.Value = supplierZoneServicesOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }

        internal static DataTable BuildRoutingSupplierInfoTable(IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            DataTable dtRoutingInfos = GetRoutingSupplierInfoTable();
            dtRoutingInfos.BeginLoadData();
            foreach (var supplierInfo in supplierInfos)
            {
                DataRow drSupplier = dtRoutingInfos.NewRow();
                drSupplier["SupplierId"] = supplierInfo.SupplierId;
                dtRoutingInfos.Rows.Add(drSupplier);
            }
            dtRoutingInfos.EndLoadData();

            return dtRoutingInfos;
        }
        private static DataTable GetRoutingSupplierInfoTable()
        {
            DataTable dtRoutingInfos = new DataTable();
            dtRoutingInfos.Columns.Add("SupplierId", typeof(Int32));
            return dtRoutingInfos;
        }
       
        #endregion
       
        #region Mappers
        
        SupplierZoneService SupplierZoneServiceMapper(IDataReader reader)
        {
            return new SupplierZoneService()
            {
                SupplierZoneServiceId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                SupplierId = GetReaderValue<int>(reader, "SupplierID"),
                ReceivedServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["ReceivedServicesFlag"] as string),
                EffectiveServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["EffectiveServiceFlag"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
        }


        SupplierDefaultService SupplierDefaultServiceMapper(IDataReader reader)
        {
            return new SupplierDefaultService()
            {
                SupplierZoneServiceId = (long)reader["ID"],
                SupplierId = GetReaderValue<int>(reader, "SupplierID"),
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                ReceivedServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["ReceivedServicesFlag"] as string),
                EffectiveServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["EffectiveServiceFlag"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
        }

        #endregion

        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierZoneService] WITH (TABLOCK)
                                            SELECT zs.[ID], zs.[ZoneID], zs.[PriceListID], zs.[ReceivedServicesFlag], zs.[EffectiveServiceFlag], zs.[BED], zs.[EED], zs.[SourceID],  {1} AS StateBackupID  FROM [TOneWhS_BE].[SupplierZoneService] zs
                                            WITH (NOLOCK)  Inner Join [TOneWhS_BE].[SupplierZone] sz WITH (NOLOCK)  on sz.ID = zs.ZoneID
                                            Where sz.SupplierID = {2} and zs.ZoneID is not null", backupDatabase, stateBackupId, supplierId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierZoneService] ([ID], [ZoneID], [ReceivedServicesFlag], [EffectiveServiceFlag], [BED], [EED], [SourceID], [PriceListID])
                                            SELECT [ID], [ZoneID], [ReceivedServicesFlag], [EffectiveServiceFlag], [BED], [EED], [SourceID], [PriceListID] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierZoneService]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }


        public string GetDeleteCommandsBySupplierId(int supplierId)
        {
            return String.Format(@"DELETE zs FROM [TOneWhS_BE].[SupplierZoneService] zs Inner Join [TOneWhS_BE].[SupplierZone] sz on sz.ID = zs.ZoneID
                                            Where sz.SupplierID = {0} and zs.ZoneID is not null", supplierId);
        }

        #endregion
    }
}
