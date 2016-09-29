using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DataTable dtRoutingInfos = GetRoutingOwnerInfoTable();
            dtRoutingInfos.BeginLoadData();
            foreach (var c in customerInfos)
            {
                DataRow drCustomer = dtRoutingInfos.NewRow();
                drCustomer["OwnerId"] = c.CustomerId;
                drCustomer["OwnerTpe"] = (int)SalePriceListOwnerType.Customer;
                dtRoutingInfos.Rows.Add(drCustomer);

                DataRow drSP = dtRoutingInfos.NewRow();
                drSP["OwnerId"] = c.SellingProductId;
                drSP["OwnerTpe"] = (int)SalePriceListOwnerType.SellingProduct;
                dtRoutingInfos.Rows.Add(drSP);
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
            return GetItemsSP("[TOneWhS_BE].[sp_SaleEntityService_GetFiltered]", SaleEntityZoneServiceMapper,query.EffectiveOn,query.SellingNumberPlanId, zonesids,query.OwnerType,query.OwnerId );
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

        #endregion
    }
}
