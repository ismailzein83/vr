using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Data.SQL;
using TOne.LCR.Entities;

namespace TOne.LCR.Data.SQL
{    
    public class RateDataManager : BaseTOneDataManager, IRateDataManager
    {
        public void LoadZoneRates(DateTime effectiveDate, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable)
        {
            var customersZoneRates = new List<ZoneRate>();
            var suppliersZoneRates = new List<ZoneRate>();
            ExecuteReaderSP("LCR.sp_Rate_GetZoneRates",
                (reader) =>
                {
                    while(reader.Read())
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
                        if(supplierId == "SYS")
                        {
                            zoneRate.CarrierAccountId = customerId;
                            customersZoneRates.Add(zoneRate);
                        }
                        else
                        {
                            zoneRate.CarrierAccountId = supplierId;
                            suppliersZoneRates.Add(zoneRate);
                        }
                        if(suppliersZoneRates.Count >= batchSize)
                        {
                            onBatchAvailable(new ZoneRateBatch
                            {
                                IsSupplierZoneRateBatch = true,
                                ZoneRates = suppliersZoneRates
                            });
                            suppliersZoneRates = new List<ZoneRate>();
                        }
                        else if(customersZoneRates.Count >= batchSize)
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
                }, effectiveDate);
        }
    }
}