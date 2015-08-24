using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class CustomerPricelistsDataManager : BaseTOneDataManager, ICustomerPricelistsDataManager
    {
       public Vanrise.Entities.BigResult<CustomerPriceLists> GetCustomerPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[Analytics].[sp_PriceList_GetByCustomerID]", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, CustomerPriceListsMapper);
        }
       private CustomerPriceLists CustomerPriceListsMapper(IDataReader reader)
       {
           CustomerPriceLists customerPriceLists = new CustomerPriceLists
           {
               PriceListID = (int)reader["PriceListID"],
               Description = reader["Description"] as string,
               SourceFileName = reader["SourceFileName"] as string,
               UserID = GetReaderValue<int>(reader, "UserID"),
               BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
               Name = reader["Name"] as string,
           };

           return customerPriceLists;
       }

       public Vanrise.Entities.BigResult<CustomerPriceListDetail> GetCustomerPriceListDetails(Vanrise.Entities.DataRetrievalInput<int> input)
       {

           Action<string> createTempTableAction = (tempTableName) =>
           {
               ExecuteNonQuerySP("[Analytics].[sp_Rate_GetByPriceListID]", tempTableName, input.Query);
           };

           return RetrieveData(input, createTempTableAction, CustomerPriceListDetailMapper);
       }
       private CustomerPriceListDetail CustomerPriceListDetailMapper(IDataReader reader)
       {
           CustomerPriceListDetail customerPriceLists = new CustomerPriceListDetail
           {
               RateID = GetReaderValue<Int64>(reader, "RateID"),
               PriceListID = GetReaderValue<int>(reader, "PriceListID"),
               ZoneID= GetReaderValue<int>(reader, "ZoneID"),
               ZoneName = reader["ZoneName"] as string,
               Rate= GetReaderValue<Decimal>(reader, "Rate"),
               OffPeakRate= GetReaderValue<Decimal>(reader, "OffPeakRate"),
               WeekendRate= GetReaderValue<Decimal>(reader, "WeekendRate"),
               Change = (Change)GetReaderValue<Int16>(reader, "Change"),
               ServicesFlag = GetReaderValue<Int16>(reader, "ServicesFlag"),
               BeginEffectiveDate=GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
               EndEffectiveDate=GetReaderValue<DateTime>(reader, "EndEffectiveDate"),
               CodeGroup=GetReaderValue<string>(reader, "CodeGroup"),
               Notes= reader["Notes"] as string,

           };

           return customerPriceLists;
       }

    }

}
