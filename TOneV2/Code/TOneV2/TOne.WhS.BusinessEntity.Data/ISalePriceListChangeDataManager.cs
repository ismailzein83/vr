using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISalePriceListChangeDataManager : IDataManager
    {
        List<SalePricelistCodeChange> GetFilteredSalePricelistCodeChanges(int pricelistId, List<int> countryIds);
        List<SalePricelistRateChange> GetFilteredSalePricelistRateChanges(int pricelistId, List<int> countryIds);
        List<SalePricelistRPChange> GetFilteredSalePriceListRPChanges(int pricelistId, List<int> countryIds);
        SalePriceListSnapShot GetSalePriceListSnapShot(int priceListId);
        void SaveCustomerChangesToDb(IEnumerable<SalePriceListCustomerChange> salePriceLists);
        void SaveCustomerCodeChangesToDb(IEnumerable<SalePricelistCodeChange> codeChanges);
        void SaveCustomerRoutingProductChangesToDb(IEnumerable<SalePricelistRPChange> routingProductChanges, long processInstanceId);
        void SaveCustomerRateChangesToDb(IEnumerable<SalePricelistRateChange> rateChanges, long processInstanceId);
        void SaveSalePriceListSnapshotToDb(IEnumerable<SalePriceListSnapShot> salePriceListSaleCodeSnapshots);
        List<SalePricelistCodeChange> GetNotSentCodechanges(IEnumerable<int> customerIds);
        List<Entities.SalePriceListNew> GetTemporaryPriceLists(TemporarySalePriceListQuery query);
        List<SalePricelistRateChange> GetNotSentRatechanges(IEnumerable<int> customerIds);
        IEnumerable<CustomerRatePreview> GetCustomerRatePreviews(CustomerRatePreviewQuery query);
        IEnumerable<RoutingProductPreview> GetRoutingProductPreviews(RoutingProductPreviewQuery query);
        IEnumerable<int> GetAffectedCustomerIdsRPChangesByProcessInstanceId(long ProcessInstanceId);
        IEnumerable<int> GetAffectedCustomerIdsNewCountryChangesByProcessInstanceId(long ProcessInstanceId);
        IEnumerable<int> GetAffectedCustomerIdsChangedCountryChangesByProcessInstanceId(long ProcessInstanceId);
        IEnumerable<int> GetAffectedCustomerIdsRateChangesByProcessInstanceId(long ProcessInstanceId);
        bool AreSalePriceListCodeSnapShotUpdated(ref object updateHandle);
        bool DoCustomerTemporaryPricelistsExists(long processInstanceId);
        
    }
}
