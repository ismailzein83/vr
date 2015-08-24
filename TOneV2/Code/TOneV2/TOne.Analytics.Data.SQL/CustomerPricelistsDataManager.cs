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



    }

}
