﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleEntityServiceDataManager : BaseSQLDataManager, ISaleEntityServiceDataManager
    {
        public SaleEntityServiceDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public IEnumerable<SaleEntityDefaultService> GetEffectiveSaleEntityDefaultServices(DateTime? effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetEffectiveDefaultServices", SaleEntityDefaultServiceMapper, effectiveOn);
        }

        public IEnumerable<SaleEntityZoneService> GetSaleZonesServicesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityZoneServices_GetEffectiveBySellingNumberPlan", SaleEntityZoneServiceMapper, sellingNumberPlanId, effectiveOn);
        }

        public IEnumerable<SaleZoneRoutingProduct> GetSaleZoneRoutingProductsEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityZoneRoutingProducts_GetEffectiveBySellingNumberPlan", SalezoneRoutingProductMapper, sellingNumberPlanId, effectiveOn);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServices(Entities.SalePriceListOwnerType ownerType, int ownerId, DateTime? effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetEffectiveZoneServices", SaleEntityZoneServiceMapper, ownerType, ownerId, effectiveOn);
        }
        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServicesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable saleEntityServicesOwners = BuildRoutingOwnerInfoTable(customerInfos);
            return GetItemsSPCmd("TOneWhS_BE.sp_SaleEntityService_GetEffectiveZoneServicesByOwner", SaleEntityZoneServiceMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@SaleEntityServicesOwners", SqlDbType.Structured);
                dtPrm.Value = saleEntityServicesOwners;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }
        internal static DataTable BuildRoutingOwnerInfoTable(IEnumerable<RoutingCustomerInfoDetails> customerInfos)
        {
            HashSet<int> addedSellingProductIds = new HashSet<int>();
            DataTable dtRoutingInfos = GetRoutingOwnerInfoTable();
            dtRoutingInfos.BeginLoadData();
            foreach (var c in customerInfos)
            {
                DataRow drCustomer = dtRoutingInfos.NewRow();
                drCustomer["OwnerId"] = c.CustomerId;
                drCustomer["OwnerTpe"] = (int)SalePriceListOwnerType.Customer;
                dtRoutingInfos.Rows.Add(drCustomer);

                if (addedSellingProductIds.Contains(c.SellingProductId))
                    continue;

                DataRow drSP = dtRoutingInfos.NewRow();
                drSP["OwnerId"] = c.SellingProductId;
                drSP["OwnerTpe"] = (int)SalePriceListOwnerType.SellingProduct;
                dtRoutingInfos.Rows.Add(drSP);
                addedSellingProductIds.Add(c.SellingProductId);
            }
            dtRoutingInfos.EndLoadData();
            return dtRoutingInfos;
        }
        private static DataTable GetRoutingOwnerInfoTable()
        {
            DataTable dtRoutingInfos = new DataTable();
            dtRoutingInfos.Columns.Add("OwnerId", typeof(Int32));
            dtRoutingInfos.Columns.Add("OwnerTpe", typeof(Int32));
            return dtRoutingInfos;
        }


        public IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetDefaultServicesEffectiveAfter", SaleEntityDefaultServiceMapper, ownerType, ownerId, minimumDate);
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetZoneServicesEffectiveAfter", SaleEntityZoneServiceMapper, ownerType, ownerId, minimumDate);
        }
        public bool AreSaleEntityServicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleEntityService", ref updateHandle);
        }

        public IEnumerable<SaleEntityZoneService> GetFilteredSaleEntityZoneService(SaleEntityZoneServiceQuery query)
        {
            string zonesids = null;
            if (query.ZonesIds != null && query.ZonesIds.Count() > 0)
                zonesids = string.Join<long>(",", query.ZonesIds);
            return GetItemsSP("[TOneWhS_BE].[sp_SaleEntityService_GetFiltered]", SaleEntityZoneServiceMapper, query.EffectiveOn, query.SellingNumberPlanId, zonesids, query.OwnerType, query.OwnerId);
        }


        #region Mappers

        private SaleEntityDefaultService SaleEntityDefaultServiceMapper(IDataReader reader)
        {
            return new SaleEntityDefaultService()
            {
                SaleEntityServiceId = (long)reader["ID"],
                PriceListId = (int)reader["PriceListID"],
                Services = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["Services"] as string),
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        private SaleEntityZoneService SaleEntityZoneServiceMapper(IDataReader reader)
        {
            return new SaleEntityZoneService()
            {
                SaleEntityServiceId = (long)reader["ID"],
                PriceListId = (int)reader["PriceListID"],
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                Services = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["Services"] as string),
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        private SaleZoneRoutingProduct SalezoneRoutingProductMapper(IDataReader reader)
        {
            return new SaleZoneRoutingProduct()
            {
                SaleEntityRoutingProductId = (long)reader["ID"],
                RoutingProductId = (int)reader["RoutingProductID"],
                OwnerId = (int)reader["OwnerID"],
                OwnerType = (SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                SaleZoneId = (long)reader["ZoneID"],
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        #endregion


        #region State Backup Methods

        public string BackupAllSaleEntityZoneServiceDataBySellingNumberPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleEntityService] WITH (TABLOCK)
                                            SELECT ses.[ID], ses.[PriceListID], ses.[ZoneID], ses.[Services], ses.[BED], ses.[EED], ses.[SourceID], {1} AS StateBackupID,ses.[LastModifiedTime]  FROM [TOneWhS_BE].[SaleEntityService]
                                            ses WITH (NOLOCK) Inner Join [TOneWhS_BE].SaleZone sz WITH (NOLOCK) on ses.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }

        public string BackupAllSaleEntityZoneServiceDataByOwner(long stateBackupId, string backupDatabase, int ownerId, int ownerType)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleEntityService] WITH (TABLOCK)
                                            SELECT ses.[ID], ses.[PriceListID], ses.[ZoneID], ses.[Services], ses.[BED], ses.[EED], ses.[SourceID], {1} AS StateBackupID ,ses.[LastModifiedTime]  FROM [TOneWhS_BE].[SaleEntityService]
                                            ses WITH (NOLOCK) Inner Join [TOneWhS_BE].SalePriceList pl WITH (NOLOCK) on ses.PriceListID = pl.ID
                                            Where pl.OwnerId = {2} and pl.OwnerType = {3}", backupDatabase, stateBackupId, ownerId, ownerType);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleEntityService] ([ID], [PriceListID], [ZoneID], [Services], [BED], [EED], [SourceID],[LastModifiedTime])
                                            SELECT [ID], [PriceListID], [ZoneID], [Services], [BED], [EED], [SourceID],[LastModifiedTime] FROM [{0}].[TOneWhS_BE_Bkup].[SaleEntityService]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }


        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE ses FROM [TOneWhS_BE].[SaleEntityService] ses Inner Join [TOneWhS_BE].SaleZone sz on ses.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {0}", sellingNumberPlanId);
        }


        public string GetDeleteCommandsByOwner(int ownerId, int ownerType)
        {
            return String.Format(@"DELETE ses FROM [TOneWhS_BE].[SaleEntityService]
                                            ses  Inner Join [TOneWhS_BE].SalePriceList pl  on ses.PriceListID = pl.ID
                                            Where pl.OwnerId = {0} and pl.OwnerType = {1}", ownerId, ownerType);
        }

        #endregion

    }
}
