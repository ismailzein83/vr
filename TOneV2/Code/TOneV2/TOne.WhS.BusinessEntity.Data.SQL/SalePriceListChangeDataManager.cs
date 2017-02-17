using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SalePriceListChangeDataManager : BaseSQLDataManager, ISalePriceListChangeDataManager
    {
        public SalePriceListChangeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #region public Methods
        public List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds)
        {
            string strcountryIds = null;
            if (countryIds != null && countryIds.Count > 0)
                strcountryIds = string.Join(",", countryIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistCodeChange_GetFiltered", SalePricelistCodeChangeMapper,
                pricelistId, strcountryIds);
        }
        public List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds)
        {
            string strcountryIds = null;
            if (countryIds != null && countryIds.Count > 0)
                strcountryIds = string.Join(",", countryIds);
            return GetItemsSP("TOneWhS_BE.sp_SalePricelistRateChange_GetFiltered", SalePricelistRateChangeMapper,
                pricelistId, strcountryIds);
        }
        #endregion
        #region Mappers
        SalePricelistCodeChange SalePricelistCodeChangeMapper(IDataReader reader)
        {
            SalePricelistCodeChange salePricelistCodeChange = new SalePricelistCodeChange
            {
                PricelistId = (int)reader["PricelistID"],
                Code = reader["Code"] as string,
                CountryId = (int)reader["CountryID"],
                RecentZoneName = reader["RecentZoneName"] as string,
                ZoneName = reader["ZoneName"] as string,
                ChangeType = (CodeChange)GetReaderValue<byte>(reader, "Change")
            };
            return salePricelistCodeChange;
        }
        SalePricelistRateChange SalePricelistRateChangeMapper(IDataReader reader)
        {
            SalePricelistRateChange salePricelistCodeChange = new SalePricelistRateChange
            {
                PricelistId = (int)reader["PricelistID"],
                CountryId = (int)reader["CountryID"],
                ZoneName = reader["ZoneName"] as string,
                Rate = (decimal)reader["Rate"],
                RecentRate = (decimal)reader["RecentRate"],
                ChangeType = (RateChangeType)GetReaderValue<byte>(reader, "Change")
            };
            return salePricelistCodeChange;
        }

        #endregion
    }
}
