using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ISupplierPricelistsDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input);
        bool SavePriceList(int priceListStatus, DateTime effectiveOnDateTime, string supplierId, string priceListType, string activeSupplierEmail, byte[] contentBytes, string fileName, string messageUid, out int insertdId);
        int GetQueueStatus(int queueId);

    }
}
