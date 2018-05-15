using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class OtherRatesPreviewDataManager : BaseSQLDataManager, IOtherRatesPreviewDataManager
    {
        public OtherRatesPreviewDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByProcessInstanceId(int processInstanceId,string zoneName,int customerId)
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePriceListNew_GetPreviewsByProcessInstanceId", OtherRatePreviewMapper, processInstanceId, zoneName, customerId);
        }
        public IEnumerable<SalePricelistRateChange> GetFilteredRatesPreviewByPriceListId(int pricelistId,string zoneName)
        {
            return GetItemsSP("TOneWhS_BE.sp_SalePriceListNew_GetPreviewsByPricelistId", OtherRatePreviewMapper, pricelistId, zoneName);
        }
        private SalePricelistRateChange OtherRatePreviewMapper(IDataReader reader)
        {
            return new SalePricelistRateChange()
            {
                ZoneName = reader["ZoneName"] as string,
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeId"),
                RecentRate = GetReaderValue<decimal?>(reader, "RecentRate"),
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                ChangeType = (RateChangeType)GetReaderValue<byte>(reader, "Change"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
            };
        }
    }
}
