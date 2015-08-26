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
    public class BasePriceListDataManager : BaseTOneDataManager, IBasePriceListDataManager
    {
        public Vanrise.Entities.BigResult<T> GetPriceListDetails<T>(Vanrise.Entities.DataRetrievalInput<int> input) where T : BasePriceListDetail
        {

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[Analytics].[sp_Rate_GetByPriceListID]", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, SupplierPriceListDetailMapper<T>);
        }
        private T SupplierPriceListDetailMapper<T>(IDataReader reader) where T : BasePriceListDetail
        {
            T priceList = Activator.CreateInstance<T>();
            priceList.RateID = GetReaderValue<Int64>(reader, "RateID");
              priceList.PriceListID = GetReaderValue<int>(reader, "PriceListID");
              priceList.ZoneID = GetReaderValue<int>(reader, "ZoneID");
              priceList.ZoneName = reader["ZoneName"] as string;
              priceList.Rate = GetReaderValue<Decimal>(reader, "Rate");
              priceList.OffPeakRate = GetReaderValue<Decimal>(reader, "OffPeakRate");
              priceList.WeekendRate = GetReaderValue<Decimal>(reader, "WeekendRate");
              priceList.Change = (Change)GetReaderValue<Int16>(reader, "Change");
              priceList.ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag");
              priceList.BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate");
              priceList.EndEffectiveDate = GetReaderValue<DateTime>(reader, "EndEffectiveDate");
              priceList.CodeGroup = GetReaderValue<string>(reader, "CodeGroup");
              priceList.Notes = reader["Notes"] as string;
            return priceList;
        }
    }
}
