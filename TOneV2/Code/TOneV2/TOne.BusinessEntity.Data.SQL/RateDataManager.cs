using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class RateDataManager : BaseTOneDataManager, IRateDataManager
    {

        private Rate RateMapper(IDataReader reader)
        {
            return new Rate
             {
                 RateId = (long)reader["RateID"],
                 SupplierId = reader["SupplierID"] as string,
                 CustomerId = reader["CustomerID"] as string,
                 ZoneId = GetReaderValue<int>(reader, "ZoneId"),
                 NormalRate = GetReaderValue<decimal>(reader, "Rate"),
                 OffPeakRate = GetReaderValue<decimal?>(reader, "OffPeakRate"),
                 WeekendRate = GetReaderValue<decimal?>(reader, "WeekendRate"),
                 PriceListId = GetReaderValue<int>(reader, "PricelistId"),
                 ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                 BeginEffectiveDate = GetReaderValue<DateTime?>(reader, "RateBeginEffectiveDate"),
                 EndEffectiveDate = GetReaderValue<DateTime?>(reader, "RateEndEffectiveDate"),
                 CurrencyID = reader["CurrencyID"] as string,
                 CurrencyLastRate = (float)GetReaderValue<double>(reader, "CurrencyLastRate"),
                 Change = TOne.BusinessEntity.Entities.Change.None
             };
        }

        public List<Rate> GetRate(int zoneId, string customerId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Rate_GetRates", RateMapper, ToDBNullIfDefault(zoneId), customerId, when);
        }

        public void LoadCalculatedZoneRates(DateTime effectiveTime, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable)
        {
            var customersZoneRates = new List<ZoneRate>();
            var suppliersZoneRates = new List<ZoneRate>();
            DataTable dtZoneIds = BuildInfoTable<int>(new List<int>(), "ID");
            ExecuteReaderSPCmd("[BEntity].[sp_Rate_GetCaclulatedZoneRates]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        ZoneRate zoneRate = new ZoneRate
                        {
                            RateId = (long)reader["RateID"],
                            PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                            Rate = reader["NormalRate"] != DBNull.Value ? Convert.ToDecimal(reader["NormalRate"]) : 0,
                            ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                            ZoneId = GetReaderValue<int>(reader, "ZoneID"),
                            BED = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                            EED = reader["EndEffectiveDate"] == DBNull.Value ? null : (DateTime?)reader["EndEffectiveDate"]
                        };
                        string supplierId = reader["SupplierID"] as string;
                        string customerId = reader["CustomerID"] as string;
                        if (supplierId == "SYS")
                        {
                            zoneRate.CarrierAccountId = customerId;
                            customersZoneRates.Add(zoneRate);
                        }
                        else
                        {
                            zoneRate.CarrierAccountId = supplierId;
                            suppliersZoneRates.Add(zoneRate);
                        }
                        if (suppliersZoneRates.Count >= batchSize)
                        {
                            onBatchAvailable(new ZoneRateBatch
                            {
                                IsSupplierZoneRateBatch = true,
                                ZoneRates = suppliersZoneRates
                            });
                            suppliersZoneRates = new List<ZoneRate>();
                        }
                        else if (customersZoneRates.Count >= batchSize)
                        {
                            onBatchAvailable(new ZoneRateBatch
                            {
                                IsSupplierZoneRateBatch = false,
                                ZoneRates = customersZoneRates
                            });
                            customersZoneRates = new List<ZoneRate>();
                        }
                    }
                    if (suppliersZoneRates.Count >= 0)
                    {
                        onBatchAvailable(new ZoneRateBatch
                        {
                            IsSupplierZoneRateBatch = true,
                            ZoneRates = suppliersZoneRates
                        });
                    }
                    if (customersZoneRates.Count >= 0)
                    {
                        onBatchAvailable(new ZoneRateBatch
                        {
                            IsSupplierZoneRateBatch = false,
                            ZoneRates = customersZoneRates
                        });
                    }
                }, (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneIds", SqlDbType.Structured);
                    dtPrm.TypeName = "LCR.IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveTime));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                });
        }

        public void GetCalculatedZoneRates(DateTime effectiveTime, bool isFuture, IEnumerable<int> zoneIds, out List<ZoneRate> customerZoneRates, out List<ZoneRate> supplierZoneRates)
        {
            var customersZoneRatesLocal = new List<ZoneRate>();
            var suppliersZoneRatesLocal = new List<ZoneRate>();
            DataTable dtZoneIds = BuildInfoTable<int>(zoneIds, "ID");
            ExecuteReaderSPCmd("[BEntity].[sp_Rate_GetCaclulatedZoneRates]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        ZoneRate zoneRate = new ZoneRate
                        {
                            RateId = (long)reader["RateID"],
                            PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                            Rate = reader["NormalRate"] != DBNull.Value ? Convert.ToDecimal(reader["NormalRate"]) : 0,
                            ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                            ZoneId = GetReaderValue<int>(reader, "ZoneID")
                        };
                        string supplierId = reader["SupplierID"] as string;
                        string customerId = reader["CustomerID"] as string;
                        if (supplierId == "SYS")
                        {
                            zoneRate.CarrierAccountId = customerId;
                            customersZoneRatesLocal.Add(zoneRate);
                        }
                        else
                        {
                            zoneRate.CarrierAccountId = supplierId;
                            suppliersZoneRatesLocal.Add(zoneRate);
                        }


                    }

                }, (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ZoneIds", SqlDbType.Structured);
                    dtPrm.TypeName = "LCR.IntIDType";
                    dtPrm.Value = dtZoneIds;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveTime));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                });
            customerZoneRates = customersZoneRatesLocal;
            supplierZoneRates = suppliersZoneRatesLocal;
        }


        private DataTable BuildInfoTable<T>(IEnumerable<T> ids, string columnName)
        {
            DataTable dtInfoTable = new DataTable();
            dtInfoTable.Columns.Add(columnName, typeof(T));
            dtInfoTable.BeginLoadData();
            foreach (var t in ids)
            {
                DataRow dr = dtInfoTable.NewRow();
                dr[columnName] = t;
                dtInfoTable.Rows.Add(dr);
            }
            dtInfoTable.EndLoadData();
            return dtInfoTable;
        }
        public List<ExchangeRate> GetExchangeRates(DateTime Date)
        {
            return GetItemsSP("BEntity.sp_CurrencyExchangeRate_GetByExchangeDate", ExchangeRateMapper, Date);
        }
        private ExchangeRate ExchangeRateMapper(IDataReader reader)
        {
            return new ExchangeRate
            {
                CurrencyExchangeRateID = GetReaderValue<Int64>(reader, "CurrencyExchangeRateID"),
                CurrencyID = reader["CurrencyID"] as string,
                Rate = GetReaderValue<float>(reader, "Rate"),
                ExchangeDate = GetReaderValue<DateTime>(reader, "ExchangeDate"),

            };
        }


        public void SaveRates(List<Rate> rates)
        {
            DataTable dtRates = BuildRatesDataTable(rates);
            WriteDataTableToDB(dtRates, SqlBulkCopyOptions.KeepNulls);
        }

        DataTable BuildRatesDataTable(List<Rate> rates)
        {
            DataTable dtRates = new DataTable();
            dtRates.Columns.Add("PriceListID", typeof(int));
            dtRates.Columns.Add("ZoneID", typeof(int));
            dtRates.Columns.Add("Rate", typeof(decimal));
            dtRates.Columns.Add("OffPeakRate", typeof(decimal));
            dtRates.Columns.Add("WeekendRate", typeof(decimal));
            dtRates.Columns.Add("Change", typeof(short));
            dtRates.Columns.Add("ServicesFlag", typeof(short));
            dtRates.Columns.Add("BeginEffectiveDate", typeof(DateTime));
            dtRates.Columns.Add("EndEffectiveDate", typeof(DateTime));
            dtRates.Columns.Add("Notes", typeof(string));
            dtRates.Columns.Add("UserID", typeof(int));
            dtRates.TableName = "Rate";
            dtRates.BeginLoadData();
            foreach (var r in rates)
            {
                DataRow dr = dtRates.NewRow();
                dr["PriceListID"] = r.PriceListId;
                dr["ZoneID"] = r.ZoneId;
                dr["Rate"] = r.NormalRate;
                dr["OffPeakRate"] = r.OffPeakRate;
                dr["WeekendRate"] = r.WeekendRate;
                dr["Change"] = r.Change;
                dr["ServicesFlag"] = r.ServicesFlag;
                dr["BeginEffectiveDate"] = r.BeginEffectiveDate;
                dr["EndEffectiveDate"] = r.EndEffectiveDate.HasValue ? (object)r.EndEffectiveDate.Value : DBNull.Value;
                dr["UserID"] = 0;
                dr["Notes"] = "Created From Mobile.";

                dtRates.Rows.Add(dr);
            }
            dtRates.EndLoadData();
            return dtRates;

        }

        private object PrepareRatesDBApply(List<Rate> rates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var r in rates)
                {

                    wr.WriteLine(String.Format("0^{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}", r.PriceListId, r.ZoneId, r.NormalRate, r.OffPeakRate, r.WeekendRate, (short)r.Change, r.ServicesFlag, r.BeginEffectiveDate.Value.ToString("yyyy-MM-dd HH:mm:00"), r.EndEffectiveDate.HasValue ? r.EndEffectiveDate.Value.ToString("yyyy-MM-dd HH:mm:00") : "", "", "", "", ""));
                    //123^145^0^^^1^1^12/14/2015 2:26:28 PM

                }
                wr.Close();
            }

            return new BulkInsertInfo
            {
                TableName = "Rate",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };
        }


        public void UpdateRateEED(List<Rate> rates, string customerId)
        {
            DataTable dtRatesType = GetDataTable(rates, customerId);
            ExecuteNonQuerySPCmd("[BEntity].[sp_Rate_UpdateEED]", (cmd) =>
            {

                var dtPrm = new SqlParameter("@Rates", SqlDbType.Structured);
                dtPrm.TypeName = "BEntity.RateType";
                dtPrm.Value = dtRatesType;
                cmd.Parameters.Add(dtPrm);
            });

        }

        private DataTable GetDataTable(List<Rate> rates, string customerId)
        {
            DataTable dtRates = new DataTable();
            dtRates.Columns.Add("PriceListId", typeof(int));
            dtRates.Columns.Add("CustomerId", typeof(string));
            dtRates.Columns.Add("ZoneId", typeof(int));
            dtRates.Columns.Add("EED", typeof(DateTime));
            dtRates.BeginLoadData();
            foreach (Rate rate in rates)
            {
                DataRow dr = dtRates.NewRow();
                dr["PriceListId"] = rate.PriceListId;
                dr["CustomerId"] = customerId;
                dr["ZoneId"] = rate.ZoneId;
                dr["EED"] = rate.EndEffectiveDate;
                dtRates.Rows.Add(dr);
            }
            dtRates.EndLoadData();
            return dtRates;
        }
    }
}