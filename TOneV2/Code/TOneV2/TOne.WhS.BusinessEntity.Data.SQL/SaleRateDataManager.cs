﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public List<SaleRate> GetSaleRatesByZoneIds(List<long> zoneIds)
        {
            string commaSeparatedZoneIds = string.Join<long>(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByZoneIDs", SaleRateMapper, commaSeparatedZoneIds);
        }

        #region Private Methods

        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            //saleRate.RoutingProductId = GetReaderValue<int?>(reader, "RoutingProductId");
            saleRate.RoutingProductId = (int)reader["RoutingProductId"];

            saleRate.NormalRate = (decimal)reader["Rate"];
            saleRate.OtherRates = null; // what about this field?
            
            saleRate.BeginEffectiveDate = (DateTime)reader["BED"];
            saleRate.EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED");

            return saleRate;
        }

        #endregion
    }
}
