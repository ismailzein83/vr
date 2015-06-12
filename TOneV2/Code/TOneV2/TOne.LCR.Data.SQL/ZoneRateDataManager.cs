using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}", zr.PriceListId, zr.ZoneId, zr.CarrierAccountId, zr.Rate, zr.ServicesFlag));
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

        #endregion







    }
}