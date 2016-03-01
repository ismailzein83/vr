
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;

namespace CP.SupplierPricelist.Data
{
    public interface IPriceListDataManager : IDataManager
    {
        bool Insert(PriceList priceList);
        List<PriceList> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId);
        List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses);
        List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses, int customerId);
        bool UpdatePriceListUpload(long id, int result, object uploadInformation, int uploadRetryCount);
        bool UpdatePriceListProgress(long id, int status, int result, int resultRetryCount, string alertMessage);
    }
}
