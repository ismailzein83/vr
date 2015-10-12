using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class SaleZoneMarketPriceDataManager : BaseSQLDataManager, ISaleZoneMarketPriceDataManager
    {

        public IEnumerable<SaleZoneMarketPrice> GetSaleZoneMarketPrices()
        {
            return GetItemsSP("[BEntity].[sp_SaleMarketPrice_GetAll]", SaleZoneMarketPriceMapper);
        }
        public SaleZoneMarketPrices GetAllSaleZoneMarketPrices()
        {
            SaleZoneMarketPrices saleMarketPrices = new SaleZoneMarketPrices();
            ExecuteReaderSP("[BEntity].[sp_SaleMarketPrice_GetAll]", (reader) =>
            {
                while (reader.Read())
                {
                    SaleZoneMarketPrice saleZoneMarketPrice = SaleZoneMarketPriceMapper(reader);
                    string key = string.Format("{0}-{1}", saleZoneMarketPrice.ZoneId, saleZoneMarketPrice.ServiceFlag);
                    if (!saleMarketPrices.ContainsKey(key))
                        saleMarketPrices.Add(key, saleZoneMarketPrice);
                }
            });
            return saleMarketPrices;
        }
        SaleZoneMarketPrice SaleZoneMarketPriceMapper(IDataReader reader)
        {
            return new SaleZoneMarketPrice()
            {
                ZoneId = (int)reader["SaleZoneID"],
                ServiceFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                FromRate = Convert.ToDecimal(reader["FromRate"]),
                ToRate = Convert.ToDecimal(reader["ToRate"])
            };
        }

    }
}
