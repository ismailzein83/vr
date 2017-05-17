using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISalePriceListChangeDataManager : IDataManager
    {
        List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds);
        List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds);
        List<SalePricelistRPChange> GetFilteredSalePriceListRPChanges(int pricelistId, List<int> countryIds);
        void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceLists);
        void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges);
        void SaveCustomerRoutingProductChangesToDb(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId);
        void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId);
        List<SalePricelistCodeChange> GetNotSentCodechanges(IEnumerable<int> customerIds);
        List<SalePricelistRateChange> GetNotSentRatechanges(IEnumerable<int> customerIds);
    }
}
