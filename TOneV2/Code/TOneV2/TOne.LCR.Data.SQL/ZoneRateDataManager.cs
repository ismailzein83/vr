using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using TOne.LCR.Entities;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class ZoneRateDataManager : RoutingDataManager, IZoneRateDataManager
    {

        public void InsertZoneRates(bool isSupplierZoneRates, List<ZoneRate> zoneRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var zr in zoneRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", zr.RateId, zr.PriceListId, zr.ZoneId, zr.CarrierAccountId, zr.Rate, zr.ServicesFlag, GetDateTimeForBCP(zr.BED), zr.EED.HasValue ? GetDateTimeForBCP(zr.EED.Value) : ""));
                }
                wr.Close();
            }
            var bulkInsertInfo = new BulkInsertInfo
            {
                TableName = String.Format("{0}ZoneRate", isSupplierZoneRates ? "Supplier" : "Customer"),
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
            InsertBulkToTable(bulkInsertInfo);
        }

        public ZoneRate ZoneRateMapper(IDataReader reader, bool isSupplierZoneRates)
        {
            return new ZoneRate()
            {
                RateId = (int)reader["RateID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                CarrierAccountId = reader[string.Format("{0}ID", isSupplierZoneRates ? "Supplier" : "Customer")] as string,
                Rate = GetReaderValue<decimal>(reader, "NormalRate"),
                ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag")
            };
        }

        public ZoneCustomerRates GetZoneCustomerRates(IEnumerable<Int32> lstZoneIds)
        {
            ZoneCustomerRates allCustomerZoneRates = new ZoneCustomerRates();
            DataTable dtZoneIds = BuildZoneInfoTable(lstZoneIds);
            allCustomerZoneRates.ZonesCustomersRates = new Dictionary<int, CustomerRates>();

            ExecuteReaderText(string.Format(query_GetZoneRates, "Customer"),
                   (reader) =>
                   {
                       while (reader.Read())
                       {
                           string customerId = reader["CustomerID"] as string;
                           var rate = new RateInfo
                           {
                               ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                               Rate = reader["NormalRate"] != DBNull.Value ? Convert.ToDecimal(reader["NormalRate"]) : 0,
                               ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                               PriceListId = GetReaderValue<int>(reader, "PriceListID")
                           };

                           CustomerRates customerRates;
                           if (!allCustomerZoneRates.ZonesCustomersRates.TryGetValue(rate.ZoneId, out customerRates))
                           {
                               customerRates = new CustomerRates();
                               customerRates.CustomersRates = new Dictionary<string, RateInfo>();
                               allCustomerZoneRates.ZonesCustomersRates.Add(rate.ZoneId, customerRates);
                           }

                           if (!customerRates.CustomersRates.ContainsKey(customerId))
                               customerRates.CustomersRates.Add(customerId, rate);
                       }

                   },

                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                    dtPrm.TypeName = "IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                });

            return allCustomerZoneRates;

        }

        public SupplierZoneRates GetSupplierZoneRates(IEnumerable<int> lstZoneIds)
        {
            SupplierZoneRates allSupplierZoneRates = new SupplierZoneRates();
            DataTable dtZoneIds = BuildZoneInfoTable(lstZoneIds);
            //allSupplierZoneRates.SuppliersZonesRates = new Dictionary<string, ZoneRates>();
            allSupplierZoneRates.RatesByZoneId = new Dictionary<int, RateInfo>();
            List<RateInfo> zoneRates = new List<RateInfo>();
            ExecuteReaderText(string.Format(query_GetZoneRates, "Supplier"),
                 (reader) =>
                 {
                     while (reader.Read())
                     {
                         var rate = new RateInfo
                         {
                             ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                             Rate = reader["NormalRate"] != DBNull.Value ? Convert.ToDecimal(reader["NormalRate"]) : 0,
                             ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                             PriceListId = GetReaderValue<int>(reader, "PriceListID")
                         };

                         if (!allSupplierZoneRates.RatesByZoneId.ContainsKey(rate.ZoneId))
                         {
                             allSupplierZoneRates.RatesByZoneId.Add(rate.ZoneId, rate);
                             zoneRates.Add(rate);
                         }
                     }

                 },

                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneList", SqlDbType.Structured);
                    dtPrm.TypeName = "IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                });
            return allSupplierZoneRates;

        }

        public CustomerSaleZones GetCustomerSaleZones(string customerId, string zoneName)
        {
            CustomerSaleZones customerSaleZones = new CustomerSaleZones();
            int previousZoneId = 0;
            CodeManager codeManager = new CodeManager();
            this.RoutingDatabaseType = Entities.RoutingDatabaseType.Current;
            ExecuteReaderText(string.Format(query_GetSaleZoneLCR, zoneName, customerId),
                (reader) =>
                {
                    while (reader.Read())
                    {
                        CustomerSaleZone customerSaleZone;
                        int zoneId = (int)reader["OurZoneId"];
                        if (!customerSaleZones.TryGetValue(zoneId, out customerSaleZone))
                        {
                            customerSaleZone = CustomerSaleZoneMapper(reader);
                            customerSaleZone.ZoneId = zoneId;
                            //customerSaleZone.EffectiveCodes = string.Join(",", codeManager.GetCodes(zoneId, DateTime.Now).Select(x => x.Value).ToArray());
                            customerSaleZones.Add(zoneId, customerSaleZone);
                            previousZoneId = zoneId;
                        }
                        SupplierLCR supplierLcr = SupplierLCRMapper(reader);
                        customerSaleZone.SuppliersLcr.Add(supplierLcr);
                    }
                }, null);

            return customerSaleZones;
        }

        private void FilterSupplierRates(CustomerSaleZone customerSaleZone)
        {
            List<SupplierLCR> sortedLCR = new List<SupplierLCR>(customerSaleZone.SuppliersLcr.OrderBy(r => r.Rate).Take(3));
            List<SupplierLCR> finalLCR = new List<SupplierLCR>();
            foreach (var supplierLCR in sortedLCR)
            {
                finalLCR.Add(supplierLCR);
            }
            customerSaleZone.SuppliersLcr = finalLCR;
        }

        private DataTable BuildZoneInfoTable(IEnumerable<int> zoneIds)
        {
            DataTable dtZoneInfo = new DataTable();
            dtZoneInfo.Columns.Add("ZoneID", typeof(Int32));
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

        CustomerSaleZone CustomerSaleZoneMapper(IDataReader reader)
        {
            return new CustomerSaleZone()
            {
                Rate = Convert.ToDecimal(reader["OurRate"]),
                ServiceFlag = GetReaderValue<short>(reader, "OurServiceFlag"),
                ZoneName = reader["OurZoneName"] as string,
                SuppliersLcr = new List<SupplierLCR>(),
                PriceListId = (int)reader["OurPriceListId"],
                RateId = (Int64)reader["OurRateId"]
            };
        }
        SupplierLCR SupplierLCRMapper(IDataReader reader)
        {
            return new SupplierLCR()
            {
                ZoneId = (int)reader["ZoneID"],
                Rate = Convert.ToDecimal(reader["NormalRate"]),
                ServiceFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                SupplierId = reader["SupplierId"] as string,
                ZoneName = reader["Name"] as string,
                IsCodeGroup = GetReaderValue<bool>(reader, "IsCodeGroup"),
                PriceListId = (int)reader["PriceListId"]
            };
        }

        #region Queries

        const string query_GetZoneRates = @" 
                                                                                       
                                            SELECT
                                           zr.[ZoneID]
	                                       ,zr.[{0}ID]
                                           ,zr.[NormalRate]
                                           ,zr.[ServicesFlag]
                                            ,zr.PriceListID
                                      FROM  [{0}ZoneRate] zr
                                        JOIN @ZoneList z ON z.ID = zr.ZoneID
                                        order by zr.ZoneID";

        const string query_GetSaleZoneLCR = @"
                                            WITH filteredZones AS (
                                            SELECT * FROM (SELECT DISTINCT  z.ZoneID,z.Name FROM 
                                               (SELECT	DISTINCT OurZoneID 
	                                            FROM	ZoneMatch with (nolock)) zm  
	                                            JOIN	ZoneInfo z WITH(NoLock) on zm.OurZoneID = z.ZoneID
	                                            WHERE	z.Name like '{0}%'
		                                            ) fz
	                                            ) 

                                            SELECT	fz.Name OurZoneName,
		                                            zm.OurZoneID,
		                                            czr.ServicesFlag OurServiceFlag,
		                                            czr.NormalRate OurRate, 
                                                    czr.PriceListId OurPriceListId,
		                                            zm.SupplierID,
		                                            zr.NormalRate, 
		                                            zr.ServicesFlag,
		                                            zr.RateID, 
		                                            zr.ZoneID, 
		                                            z.Name, 
		                                            zr.PricelistID, 
		                                            zm.IsCodeGroup,
                                                    czr.RateID OurRateId
                                            FROM	ZoneMatch zm WITH (NOLOCK) 
                                            JOIN	filteredZones fz on zm.OurZoneID = fz.ZoneID 
                                            JOIN	SupplierZoneRate zr WITH (NOLOCK) ON zm.SupplierZoneID = zr.ZoneID
                                            JOIN	CustomerZoneRate czr with (nolock) on czr.ZoneID = fz.ZoneId
                                            JOIN	ZoneInfo z WITH (NOLOCK) ON z.ZoneID = zm.SupplierZoneID 
                                            where	customerid = '{1}'
                                            ORDER BY zm.OurZoneID, zr.NormalRate";

        #endregion

    }
}