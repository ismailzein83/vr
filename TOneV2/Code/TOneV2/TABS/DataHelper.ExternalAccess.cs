using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace TABS
{
    public partial class DataHelper
    {
        /// <summary>
        /// Get a set of billing statistics 
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="supplier"></param>
        /// <param name="saleZone"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static List<Billing_Stats> GetBillingStats
            (
              CarrierAccount customer
            , CarrierAccount supplier
            , Zone saleZone
            , DateTime fromDate
            , DateTime toDate
            )
        {
            List<Billing_Stats> results = new List<Billing_Stats>();

            string SQL = @"EXEC EA_BillingSummary
	                                @FromDate = @P1,
	                                @ToDate = @P2,
	                                @CustomerID = @P3,
	                                @SupplierID = @P4,
	                                @SaleZoneID = @P5";

            IDataReader reader = ExecuteReader(SQL
                                            , fromDate.ToString("yyyy-MM-dd")
                                            , toDate.ToString("yyyy-MM-dd")
                                            , customer == null ? null : customer.CarrierAccountID
                                            , supplier == null ? null : supplier.CarrierAccountID
                                            , saleZone == null ? null : (int?)saleZone.ZoneID);


            while (reader.Read())
            {
                Billing_Stats billingstats = new Billing_Stats();

                int index = -1;
                index++; DateTime CallDate = DateTime.MinValue; if (!reader.IsDBNull(index)) CallDate = reader.GetDateTime(index);
                index++; Zone SaleZone = null; if (!reader.IsDBNull(index)) SaleZone = Zone.OwnZones.ContainsKey(reader.GetInt32(index)) ? Zone.OwnZones[reader.GetInt32(index)] : ObjectAssembler.Get<Zone>(reader.GetInt32(index));
                index++; Zone SupplierZone = null; if (!reader.IsDBNull(index)) SupplierZone = ObjectAssembler.Get<Zone>(reader.GetInt32(index));
                index++; CarrierAccount Customer = null; if (!reader.IsDBNull(index)) Customer = CarrierAccount.All.ContainsKey(reader.GetString(index)) ? CarrierAccount.All[reader.GetString(index)] : TABS.ObjectAssembler.Get<CarrierAccount>(reader.GetString(index));
                index++; CarrierAccount Supplier = null; if (!reader.IsDBNull(index)) Supplier = CarrierAccount.All.ContainsKey(reader.GetString(index)) ? CarrierAccount.All[reader.GetString(index)] : TABS.ObjectAssembler.Get<CarrierAccount>(reader.GetString(index));
                index++; double SaleRate = (!reader.IsDBNull(index)) ? reader.GetDouble(index) : 0;
                index++; double CostRate = (!reader.IsDBNull(index)) ? reader.GetDouble(index) : 0;
                index++; decimal SaleDuration = (!reader.IsDBNull(index)) ? reader.GetDecimal(index) : 0m;
                index++; decimal CostDuration = (!reader.IsDBNull(index)) ? reader.GetDecimal(index) : 0m;
                index++; double SaleNet = (!reader.IsDBNull(index)) ? reader.GetDouble(index) : 0;
                index++; double CostNet = (!reader.IsDBNull(index)) ? reader.GetDouble(index) : 0;

                billingstats.CallDate = CallDate;
                billingstats.CostZone = SupplierZone;
                billingstats.SaleZone = SaleZone;
                billingstats.Customer = Customer;
                billingstats.Supplier = Supplier;
                billingstats.Sale_Rate = (decimal)SaleRate;
                billingstats.Cost_Rate = (decimal)CostRate;
                billingstats.SaleDuration = SaleDuration;
                billingstats.CostDuration = CostDuration;
                billingstats.Cost_Nets = (decimal)CostNet;
                billingstats.Sale_Nets = (decimal)SaleNet;

                results.Add(billingstats);
            }
            return results;
        }


        /// <summary>
        /// Zone Traffic with billing infos 
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="supplierID"></param>
        /// <param name="codeGroup"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static DataTable GetZoneTraffics(
             string customerID
            , string supplierID
            , string codeGroup
            , DateTime fromDate
            , DateTime toDate
            , out decimal TotalDurations
            , out int Attempts
            )
        {
            string sql = @"EXEC [EA_TrafficStats_ZoneMonitor]
                            @FromDateTime=@P1, 
                            @ToDateTime=@P2, 
                            @CustomerID=@P3, 
                            @SupplierID=@P4,
                            @CodeGroup=@P5 ";

            DataTable ZoneTrafficTable = TABS.DataHelper.GetDataTable(sql,
                                fromDate,
                                toDate,
                                customerID,
                                supplierID,
                                codeGroup
                                );

            return MergeBillingWithStats(customerID, supplierID, codeGroup, ref fromDate, ref toDate, ZoneTrafficTable, out TotalDurations, out Attempts);
        }


        /// <summary>
        /// Zone Traffic with billing infos 
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="supplierID"></param>
        /// <param name="codeGroup"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static DataTable GetTopNDestinations(
             int TopRecords
            , string customerID
            , string supplierID
            , DateTime fromDate
            , DateTime toDate
            , bool isHighestTraffic
            , out decimal TotalDurations
            , out int Attempts
            )
        {
            string sql = @"EXEC [EA_TrafficStats_TopNDestination]
                            @FromDate=@P1, 
                            @ToDate=@P2, 
                            @CustomerID=@P3, 
                            @SupplierID=@P4,
                            @TopRecords=@P5,
                            @HighestTraffic=@p6";

            DataTable ZoneTrafficTable = TABS.DataHelper.GetDataTable(sql,
                                fromDate,
                                toDate,
                                customerID,
                                supplierID,
                                TopRecords,
                                isHighestTraffic ? "Y" : "N"
                                );

            return MergeBillingWithStatsForTopNZones(customerID, supplierID, null, ref fromDate, ref toDate, ZoneTrafficTable, out TotalDurations, out Attempts);
        }


        // Meging billing and traffic 
        private static DataTable MergeBillingWithStatsForTopNZones(string customerID, string supplierID, string codeGroup, ref DateTime fromDate, ref DateTime toDate, DataTable ZoneTrafficTable, out decimal TotalDurations, out int Attempts)
        {
            ZoneTrafficTable.Columns.Add(new DataColumn("Zone"));

            TotalDurations = 0;
            Attempts = 0;

            List<string> ZoneNamesFromTraffic = new List<string>();

            foreach (DataRow row in ZoneTrafficTable.Rows)
            {
                if (row["OurZoneID"] != DBNull.Value)
                {
                    if (TABS.Zone.OwnZones.ContainsKey(int.Parse(row["OurZoneID"].ToString())))
                        row["Zone"] = TABS.Zone.OwnZones[int.Parse(row["OurZoneID"].ToString())].Name;
                    else
                        row["Zone"] = TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(row["OurZoneID"].ToString())).Name;

                    ZoneNamesFromTraffic.Add(row["Zone"].ToString());
                }


                TotalDurations += (decimal)row["DurationsInMinutes"];
                Attempts += (int)row["Attempts"];
            }



            // adding the billing infos (rate, amount ...)
            CarrierAccount customer = null; if (customerID != null) CarrierAccount.All.TryGetValue(customerID, out customer);
            CarrierAccount supplier = null; if (supplierID != null) CarrierAccount.All.TryGetValue(supplierID, out supplier);

            if (supplier != null) return ZoneTrafficTable;

            var billings = GetBillingStats(customer, supplier, null, fromDate, toDate)
                          .Where(b => codeGroup == null || b.SaleZone.CodeGroup.Code == codeGroup)
                          .GroupBy(b => b.SaleZone.Name);

            var ZoneByName = TABS.Zone.OwnZones.Values.ToDictionary(z => z.Name);

            WebHelperLibrary.DatatableAsIEnumerable trafficdata = new WebHelperLibrary.DatatableAsIEnumerable(ZoneTrafficTable.Rows);

            List<BillingStatsView> billingStatsDetails = new List<BillingStatsView>();

            foreach (var billing in billings)
            {
                List<Billing_Stats> tempstats = new List<Billing_Stats>();

                string zonename = billing.Key;
                // decimal oldrate = 0;

                if (billing.GroupBy(b => b.Sale_Rate).Count() == 1)
                {
                    billingStatsDetails.Add
                                    (
                                           new BillingStatsView
                                           {

                                               Zone = billing.Key,
                                               OurZoneID = billing.First().SaleZone.ZoneID,
                                               FromDate = billing.Min(s => s.CallDate),
                                               ToDate = billing.Max(s => s.CallDate),
                                               Rate = billing.First().Sale_Rate,
                                               Amount = billing.Sum(s => s.Sale_Nets),
                                           }
                                     )
                                    ;
                }
                else
                {
                    var groupByRate = billing.GroupBy(b => b.Sale_Rate);

                    billingStatsDetails.AddRange(groupByRate.Select(g => new
                    BillingStatsView
                    {
                        Zone = g.First().SaleZone.Name,
                        OurZoneID = g.First().SaleZone.ZoneID,
                        FromDate = g.Min(s => s.CallDate),
                        ToDate = g.Max(s => s.CallDate),
                        Rate = g.Key,
                        Amount = g.Sum(s => s.Sale_Nets)
                    }
                    ));
                }
            }

            foreach (var detail in billingStatsDetails)
            {
                var relatedTraffic = trafficdata.Where(t =>
                                 (DateTime)t["AttemptDateTime"] >= detail.FromDate
                              && (DateTime)t["AttemptDateTime"] <= detail.ToDate
                              && t["Zone"].ToString() == detail.Zone);

                if (relatedTraffic.Count() > 0)
                {
                    detail.ACD = relatedTraffic.Average(t => (decimal)t["ACD"]);
                    detail.SuccesfulAttempts = relatedTraffic.Sum(t => (int)t["SuccesfulAttempts"]);
                    detail.FailedAttempts = relatedTraffic.Sum(t => (int)t["FailedAttempts"]);
                    detail.Attempts = relatedTraffic.Sum(t => (int)t["Attempts"]);
                    detail.DeliveredAttempts = relatedTraffic.Sum(t => (int)t["DeliveredAttempts"]);
                    detail.ASR = ((decimal)detail.SuccesfulAttempts / (decimal)detail.Attempts) * 100;
                    detail.DeliveredASR = ((decimal)detail.DeliveredAttempts / (decimal)detail.Attempts) * 100;
                    detail.DurationsInMinutes = relatedTraffic.Sum(t => (decimal)t["DurationsInMinutes"]);

                    detail.AveragePDD = relatedTraffic.Average(t => (decimal)t["AveragePDD"]);
                    detail.MaxDuration = relatedTraffic.Max(t => (decimal)t["MaxDuration"]);
                }
            }

            List<BillingStatsView> finalresult = new List<BillingStatsView>();

            foreach (var zone in ZoneNamesFromTraffic)
            {
                BillingStatsView detail = null;

                if (billingStatsDetails.Any(b => b.Zone == zone))
                {
                    var all = billingStatsDetails.Where(b => b.Zone == zone);
                    foreach (var b in all)
                        finalresult.Add(b);
                }
                else
                    detail = null;

                if (detail == null)
                {
                    detail = new BillingStatsView();
                    detail.Zone = zone;
                    detail.FromDate = fromDate;
                    detail.ToDate = toDate;
                    var relatedTraffic = trafficdata.Where(t => t["Zone"].ToString() == detail.Zone);

                    if (relatedTraffic.Count() > 0)
                    {
                        detail.ACD = relatedTraffic.Average(t => (decimal)t["ACD"]);
                        detail.SuccesfulAttempts = relatedTraffic.Sum(t => (int)t["SuccesfulAttempts"]);
                        detail.FailedAttempts = relatedTraffic.Sum(t => (int)t["FailedAttempts"]);
                        detail.Attempts = relatedTraffic.Sum(t => (int)t["Attempts"]);
                        detail.DeliveredAttempts = relatedTraffic.Sum(t => (int)t["DeliveredAttempts"]);
                        detail.ASR = ((decimal)detail.SuccesfulAttempts / (decimal)detail.Attempts) * 100;
                        detail.DeliveredASR = ((decimal)detail.DeliveredAttempts / (decimal)detail.Attempts) * 100;
                        detail.DurationsInMinutes = relatedTraffic.Sum(t => (decimal)t["DurationsInMinutes"]);

                        detail.AveragePDD = relatedTraffic.Average(t => (decimal)t["AveragePDD"]);
                        detail.MaxDuration = relatedTraffic.Max(t => (decimal)t["MaxDuration"]);
                    }

                    finalresult.Add(detail);
                }
            }

            WebHelperLibrary.ObjectDataTable<BillingStatsView> results = new WebHelperLibrary.ObjectDataTable<BillingStatsView>(finalresult);

            return results;
        }

        // Meging billing and traffic 
        private static DataTable MergeBillingWithStats(string customerID, string supplierID, string codeGroup, ref DateTime fromDate, ref DateTime toDate, DataTable ZoneTrafficTable, out decimal TotalDurations, out int Attempts)
        {
            ZoneTrafficTable.Columns.Add(new DataColumn("Zone"));

            TotalDurations = 0;
            Attempts = 0;

            List<string> ZoneNamesFromTraffic = new List<string>();

            foreach (DataRow row in ZoneTrafficTable.Rows)
            {
                if (row["OurZoneID"] != DBNull.Value)
                {
                    if (TABS.Zone.OwnZones.ContainsKey(int.Parse(row["OurZoneID"].ToString())))
                        row["Zone"] = TABS.Zone.OwnZones[int.Parse(row["OurZoneID"].ToString())].Name;
                    else
                        row["Zone"] = TABS.ObjectAssembler.Get<TABS.Zone>(int.Parse(row["OurZoneID"].ToString())).Name;

                    ZoneNamesFromTraffic.Add(row["Zone"].ToString());
                }
            }

            // adding the billing infos (rate, amount ...)
            CarrierAccount customer = null; if (customerID != null) CarrierAccount.All.TryGetValue(customerID, out customer);
            CarrierAccount supplier = null; if (supplierID != null) CarrierAccount.All.TryGetValue(supplierID, out supplier);

            if (supplier != null) return ZoneTrafficTable;

            var billings = GetBillingStats(customer, supplier, null, fromDate, toDate)
                          .Where(b => codeGroup == null || b.SaleZone.CodeGroup.Code == codeGroup)
                          .GroupBy(b => b.SaleZone.Name);

            Dictionary<string, Zone> ZoneByName = new Dictionary<string, Zone>();
            foreach (var item in TABS.Zone.OwnZones.Values)
                ZoneByName[item.Name] = item;


            WebHelperLibrary.DatatableAsIEnumerable trafficdata = new WebHelperLibrary.DatatableAsIEnumerable(ZoneTrafficTable.Rows);

            List<BillingStatsView> billingStatsDetails = new List<BillingStatsView>();

            foreach (var billing in billings)
            {
                List<Billing_Stats> tempstats = new List<Billing_Stats>();

                string zonename = billing.Key;
                // decimal oldrate = 0;

                if (billing.GroupBy(b => b.Sale_Rate).Count() == 1)
                {
                    billingStatsDetails.Add
                                    (
                                           new BillingStatsView
                                           {

                                               Zone = billing.Key,
                                               OurZoneID = billing.First().SaleZone.ZoneID,
                                               FromDate = billing.Min(s => s.CallDate),
                                               ToDate = billing.Max(s => s.CallDate),
                                               Rate = billing.First().Sale_Rate,
                                               Amount = billing.Sum(s => s.Sale_Nets),
                                           }
                                     )
                                    ;
                }
                else
                {
                    var groupByRate = billing.GroupBy(b => b.Sale_Rate);

                    billingStatsDetails.AddRange(groupByRate.Select(g => new
                    BillingStatsView
                    {
                        Zone = g.First().SaleZone.Name,
                        OurZoneID = g.First().SaleZone.ZoneID,
                        FromDate = g.Min(s => s.CallDate),
                        ToDate = g.Max(s => s.CallDate),
                        Rate = g.Key,
                        Amount = g.Sum(s => s.Sale_Nets)
                    }
                    ));
                }
            }

            foreach (var detail in billingStatsDetails)
            {
                var relatedTraffic = trafficdata.Where(t =>
                                 (DateTime)t["AttemptDateTime"] >= detail.FromDate
                              && (DateTime)t["AttemptDateTime"] <= detail.ToDate
                              && t["Zone"].ToString() == detail.Zone);
                if (relatedTraffic.Count() > 0)
                {
                    detail.ACD = relatedTraffic.Average(t => (decimal)t["ACD"]);
                    detail.SuccesfulAttempts = relatedTraffic.Sum(t => (int)t["SuccesfulAttempts"]);
                    detail.FailedAttempts = relatedTraffic.Sum(t => (int)t["FailedAttempts"]);
                    detail.Attempts = relatedTraffic.Sum(t => (int)t["Attempts"]);
                    detail.DeliveredAttempts = relatedTraffic.Sum(t => (int)t["DeliveredAttempts"]);
                    detail.ASR = ((decimal)detail.SuccesfulAttempts / (decimal)detail.Attempts) * 100;
                    detail.DeliveredASR = ((decimal)detail.DeliveredAttempts / (decimal)detail.Attempts) * 100;
                    detail.DurationsInMinutes = relatedTraffic.Sum(t => (decimal)t["DurationsInMinutes"]);

                    detail.AveragePDD = relatedTraffic.Average(t => (decimal)t["AveragePDD"]);
                    detail.MaxDuration = relatedTraffic.Max(t => (decimal)t["MaxDuration"]);
                }
            }

            var ZoneNamesFromBilling = billingStatsDetails.Select(b => b.Zone).ToList();
            foreach (var zoneName in ZoneNamesFromTraffic.Distinct())
            {
                if (!ZoneNamesFromBilling.Contains(zoneName))
                {
                    var detail = new BillingStatsView();
                    var tGrouped = trafficdata.GroupBy(r => r["Zone"].ToString());
                    var t = tGrouped.Single(r => r.Key.Equals(zoneName));

                    detail.Zone = zoneName;
                    detail.OurZoneID = (int)t.First()["OurZoneID"];
                    detail.LastAttempt = (DateTime)t.Max(r => r["LastAttempt"]);

                    detail.ACD = (decimal)t.Average(r => (decimal)r["ACD"]);
                    detail.SuccesfulAttempts = (int)t.Sum(r => (int)r["SuccesfulAttempts"]);
                    detail.FailedAttempts = (int)t.Sum(r => (int)r["FailedAttempts"]);
                    detail.Attempts = (int)t.Sum(r => (int)r["Attempts"]);
                    detail.DeliveredAttempts = (int)t.Sum(r => (int)r["DeliveredAttempts"]);
                    detail.ASR = ((decimal)detail.SuccesfulAttempts / (decimal)detail.Attempts) * 100;
                    detail.DeliveredASR = ((decimal)detail.DeliveredAttempts / (decimal)detail.Attempts) * 100;
                    detail.DurationsInMinutes = t.Sum(r => (decimal)r["DurationsInMinutes"]);

                    detail.AveragePDD = (decimal)t.Average(r => (decimal)r["AveragePDD"]);
                    detail.MaxDuration = (decimal)t.Max(r => (decimal)r["MaxDuration"]);
                    detail.Rate = 0;
                    detail.Amount = 0;
                    detail.FromDate = fromDate;
                    detail.ToDate = toDate;

                    billingStatsDetails.Add(detail);

                }
            }



            WebHelperLibrary.ObjectDataTable<BillingStatsView> results = new WebHelperLibrary.ObjectDataTable<BillingStatsView>(billingStatsDetails);

            return results;
        }


        public static List<BillingStatsView> GetBillingStats(TABS.CarrierAccount customer, TABS.CarrierAccount supplier, DateTime fromDate, DateTime toDate)
        {
            var billings = GetBillingStats(customer, supplier, null, fromDate, toDate)
                          .GroupBy(b => b.SaleZone.Name);

            var ZoneByName = TABS.Zone.OwnZones.Values.ToDictionary(z => z.ZoneID, N=>N);

            List<BillingStatsView> billingStatsDetails = new List<BillingStatsView>();

            foreach (var billing in billings)
            {
                List<Billing_Stats> tempstats = new List<Billing_Stats>();

                string zonename = billing.Key;
                decimal oldrate = 0;

                if (billing.GroupBy(b => b.Sale_Rate).Count() == 1)
                {
                    billingStatsDetails.Add
                                    (
                                           new BillingStatsView
                                           {

                                               Zone = billing.Key,
                                               OurZoneID = billing.First().SaleZone.ZoneID,
                                               FromDate = billing.Min(s => s.CallDate),
                                               ToDate = billing.Max(s => s.CallDate),
                                               Rate = billing.First().Sale_Rate,
                                               Amount = billing.Sum(s => s.Sale_Nets),
                                           }
                                     )
                                    ;
                }
                else
                {
                    foreach (var billingdetail in billing.OrderBy(b => b.CallDate))
                    {
                        if (oldrate != 0 && billingdetail.Sale_Rate != oldrate)
                        {
                            billingStatsDetails.AddRange
                                          (
                                         tempstats.GroupBy(t => t.SaleZone.Name)
                                         .Select(t => new BillingStatsView
                                         {
                                             Zone = t.Key,
                                             OurZoneID = t.First().SaleZone.ZoneID,
                                             FromDate = t.Min(s => s.CallDate),
                                             ToDate = t.Max(s => s.CallDate),
                                             Rate = t.First().Sale_Rate,
                                             Amount = t.Sum(s => s.Sale_Nets),
                                         }
                                                )
                                          );

                            oldrate = billingdetail.Sale_Rate;
                            tempstats = new List<Billing_Stats>();
                        }

                        Billing_Stats stats = new Billing_Stats();

                        stats.CallDate = billingdetail.CallDate;
                        stats.SaleZone = ZoneByName.Values.Where(z => z.Name.Equals(zonename)).FirstOrDefault();
                        stats.Customer = billingdetail.Customer;
                        stats.Sale_Rate = billingdetail.Sale_Rate;
                        stats.Sale_Nets = billingdetail.Sale_Nets;

                        tempstats.Add(stats);
                    }
                }
            }


            return billingStatsDetails;

        }

        /// <summary>
        /// helper class to display the data
        /// </summary>
        public class BillingStatsView
        {
            // billing related 
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public string Zone { get; set; }
            public int OurZoneID { get; set; }
            public decimal Rate { get; set; }
            public decimal Amount { get; set; }

            //traffic related 
            public decimal ASR { get; set; }
            public decimal ACD { get; set; }
            public decimal DurationsInMinutes { get; set; }
            public decimal MaxDuration { get; set; }
            public int Attempts { get; set; }
            public decimal DeliveredASR { get; set; }
            public decimal AveragePDD { get; set; }
            public DateTime LastAttempt { get; set; }
            public int SuccesfulAttempts { get; set; }
            public int FailedAttempts { get; set; }
            public int DeliveredAttempts { get; set; }
        }


    }
}
