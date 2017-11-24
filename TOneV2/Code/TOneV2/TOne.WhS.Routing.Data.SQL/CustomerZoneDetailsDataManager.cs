using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerZoneDetailsDataManager : RoutingDataManager, ICustomerZoneDetailsDataManager
    {
        public DateTime? EffectiveDate { get; set; }
        public bool? IsFuture { get; set; }

        readonly string[] columns = { "CustomerId", "SaleZoneId", "RoutingProductId", "RoutingProductSource", "SellingProductId", "EffectiveRateValue", "RateSource", "SaleZoneServiceIds", "SaleRateTypeRuleId", "SaleRateTypeId", "VersionNumber" };
        public void SaveCustomerZoneDetailsToDB(List<CustomerZoneDetail> customerZoneDetails)
        {
            Object dbApplyStream = InitialiazeStreamForDBApply();
            foreach (CustomerZoneDetail customerZoneDetail in customerZoneDetails)
                WriteRecordToStream(customerZoneDetail, dbApplyStream);
            Object preparedCustomerZoneDetails = FinishDBApplyStream(dbApplyStream);
            ApplyCustomerZoneDetailsToDB(preparedCustomerZoneDetails);
        }
        public void ApplyCustomerZoneDetailsToDB(object preparedCustomerZoneDetails)
        {
            InsertBulkToTable(preparedCustomerZoneDetails as BaseBulkInsertInfo);
        }
        public IEnumerable<Entities.CustomerZoneDetail> GetCustomerZoneDetails()
        {
            return GetItemsText(query_GetCustomerZoneDetails, CustomerZoneDetailMapper, null);
        }
        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[dbo].[CustomerZoneDetail]",
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }
        public void WriteRecordToStream(Entities.CustomerZoneDetail record, object dbApplyStream)
        {
            string saleZoneServiceIds = record.SaleZoneServiceIds != null ? string.Join(",", record.SaleZoneServiceIds) : null;

            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}", record.CustomerId, record.SaleZoneId, record.RoutingProductId, (int)record.RoutingProductSource,
                record.SellingProductId, decimal.Round(record.EffectiveRateValue, 8), (int)record.RateSource, saleZoneServiceIds, record.SaleRateTypeRuleId, record.SaleRateTypeId, record.VersionNumber);
        }
        CustomerZoneDetail CustomerZoneDetailMapper(IDataReader reader)
        {
            return new CustomerZoneDetail()
            {
                CustomerId = (int)reader["CustomerId"],
                EffectiveRateValue = GetReaderValue<decimal>(reader, "EffectiveRateValue"),
                RateSource = GetReaderValue<SalePriceListOwnerType>(reader, "RateSource"),
                RoutingProductId = GetReaderValue<int>(reader, "RoutingProductId"),
                RoutingProductSource = GetReaderValue<SaleEntityZoneRoutingProductSource>(reader, "RoutingProductSource"),
                SaleZoneId = (Int64)reader["SaleZoneId"],
                SellingProductId = GetReaderValue<int>(reader, "SellingProductId"),
                SaleZoneServiceIds = new HashSet<int>((reader["SaleZoneServiceIds"] as string).Split(',').Select(itm => int.Parse(itm))),
                SaleRateTypeRuleId = GetReaderValue<int?>(reader, "SaleRateTypeRuleId"),
                SaleRateTypeId = GetReaderValue<int?>(reader, "SaleRateTypeId"),
                VersionNumber = (int)reader["VersionNumber"]
            };
        }

        public IEnumerable<CustomerZoneDetail> GetFilteredCustomerZoneDetailsByZone(IEnumerable<long> saleZoneIds)
        {
            DataTable dtZoneIds = BuildZoneIdsTable(new HashSet<long>(saleZoneIds));
            return GetItemsText(query_GetFilteredCustomerZoneDetailsByZone, CustomerZoneDetailMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "LongIDType";
                dtPrm.Value = dtZoneIds;
                cmd.Parameters.Add(dtPrm);
            });
        }

        public List<CustomerZoneDetail> GetCustomerZoneDetails(HashSet<CustomerSaleZone> customerSaleZones)
        {
            DataTable dtCustomerZones = BuildCustomerZonesTable(customerSaleZones);
            return GetItemsText(query_GetFilteredCustomerZoneDetailsByCustomerZone, CustomerZoneDetailMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@CustomerZoneList", SqlDbType.Structured);
                dtPrm.TypeName = "CustomerZoneType";
                dtPrm.Value = dtCustomerZones;
                cmd.Parameters.Add(dtPrm);
            });
        }

        DataTable BuildZoneIdsTable(HashSet<long> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int64));
            dtZoneInfo.BeginLoadData();
            foreach (var z in zoneIds)
            {
                DataRow dr = dtZoneInfo.NewRow();
                dr["ZoneID"] = z;
                dtZoneInfo.Rows.Add(dr);
            }
            dtZoneInfo.EndLoadData();
            return dtZoneInfo;
        }

        DataTable BuildCustomerZonesTable(HashSet<CustomerSaleZone> customerSaleZones)
        {
            DataTable dtCustomerZones = new DataTable();
            dtCustomerZones.Columns.Add("CustomerId", typeof(Int32));
            dtCustomerZones.Columns.Add("SaleZoneId", typeof(Int64));
            dtCustomerZones.BeginLoadData();
            foreach (var customerSaleZone in customerSaleZones)
            {
                DataRow dr = dtCustomerZones.NewRow();
                dr["CustomerId"] = customerSaleZone.CustomerId;
                dr["SaleZoneId"] = customerSaleZone.SaleZoneId;
                dtCustomerZones.Rows.Add(dr);
            }
            dtCustomerZones.EndLoadData();
            return dtCustomerZones;
        }

        #region Queries

        const string query_GetCustomerZoneDetails = @"                                                       
                                            SELECT zd.[CustomerId]
                                                  ,zd.[SaleZoneId]
                                                  ,zd.[RoutingProductId]
                                                  ,zd.[RoutingProductSource]
                                                  ,zd.[SellingProductId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[RateSource]
                                                  ,zd.[SaleZoneServiceIds]
                                                  ,zd.[SaleRateTypeRuleId]
                                                  ,zd.[SaleRateTypeId]
                                                  ,zd.[VersionNumber]
                                              FROM [dbo].[CustomerZoneDetail] zd with(nolock)";

        const string query_GetFilteredCustomerZoneDetailsByZone = @"                                                       
                                            SELECT zd.[CustomerId]
                                                  ,zd.[SaleZoneId]
                                                  ,zd.[RoutingProductId]
                                                  ,zd.[RoutingProductSource]
                                                  ,zd.[SellingProductId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[RateSource]
                                                  ,zd.[SaleZoneServiceIds]
                                                  ,zd.[SaleRateTypeRuleId]
                                                  ,zd.[SaleRateTypeId]
                                                  ,zd.[VersionNumber]
                                              FROM [dbo].[CustomerZoneDetail] zd with(nolock)
                                              JOIN @ZoneList z ON z.ID = zd.SaleZoneID";

        const string query_GetFilteredCustomerZoneDetailsByCustomerZone = @"                                                       
                                            SELECT zd.[CustomerId]
                                                  ,zd.[SaleZoneId]
                                                  ,zd.[RoutingProductId]
                                                  ,zd.[RoutingProductSource]
                                                  ,zd.[SellingProductId]
                                                  ,zd.[EffectiveRateValue]
                                                  ,zd.[RateSource]
                                                  ,zd.[SaleZoneServiceIds]
                                                  ,zd.[SaleRateTypeRuleId]
                                                  ,zd.[SaleRateTypeId]
                                                  ,zd.[VersionNumber]
                                              FROM [dbo].[CustomerZoneDetail] zd with(nolock)
                                              JOIN @CustomerZoneList cz ON cz.CustomerId = zd.CustomerId and  cz.SaleZoneId = zd.SaleZoneId";

        #endregion
    }
}
