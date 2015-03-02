using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

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
                CurrencyLastRate = GetReaderValue<float>(reader, "CurrencyLastRate"),
                Change = TOne.BusinessEntity.Entities.Change.None
            };
        }

        public List<Rate> GetRate(int zoneId,  string customerId, DateTime when)
        {
            return GetItemsSP("BEntity.sp_Rate_GetRates", RateMapper, zoneId, customerId, when);
        }

        public void LoadCalculatedZoneRates(DateTime effectiveTime, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable)
        {
            var customersZoneRates = new List<ZoneRate>();
            var suppliersZoneRates = new List<ZoneRate>();
            ExecuteReaderSP("[BEntity].[sp_Rate_GetCaclulatedZoneRates]",
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
                }, effectiveTime, isFuture);
        }
    }
}