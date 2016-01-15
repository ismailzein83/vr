
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;

namespace CP.SupplierPricelist.Data
{
    public interface IPriceListDataManager : IDataManager
    {
        bool Insert(PriceList priceList);
        List<PriceList> GetUpdated(ref byte[] maxTimeStamp, int nbOfRows, int userId);
        List<PriceList> GetPriceLists(List<PriceListStatus> listStatuses);
        bool UpdateInitiatePriceList(long priceListId, int status, int result);
    }
}
