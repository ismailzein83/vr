using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class SupplierPricelistsDataManager : BaseTOneDataManager, ISupplierPricelistsDataManager
    {
        public Vanrise.Entities.BigResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[Analytics].[sp_PriceList_GetBySupplierID]", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, SupplierPriceListsMapper);
        }
        private PriceLists SupplierPriceListsMapper(IDataReader reader)
        {
            PriceLists suppluerPriceLists = new PriceLists
            {
                PriceListID = (int)reader["PriceListID"],
                Description = reader["Description"] as string,
                SourceFileName = reader["SourceFileName"] as string,
                UserID = GetReaderValue<int>(reader, "UserID"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BeginEffectiveDate"),
                Name = reader["Name"] as string,
            };

            return suppluerPriceLists;
        }

      
    }
}
