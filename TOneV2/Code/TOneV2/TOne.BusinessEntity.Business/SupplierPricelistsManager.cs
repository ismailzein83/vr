using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;


namespace TOne.BusinessEntity.Business
{
    public class SupplierPricelistsManager : BasePricelistManager<SupplierPriceListDetail>
    {
        public Vanrise.Entities.IDataRetrievalResult<PriceLists> GetSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<string> input)
        {

            ISupplierPricelistsDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetSupplierPriceLists(input));
        }
        public bool SavePriceList(int priceListStatus, DateTime effectiveOnDateTime, string supplierId, string priceListType, string activeSupplierEmail, byte[] contentBytes, string fileName, out int insertdId)
        {
            ISupplierPricelistsDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return dataManager.SavePriceList(priceListStatus, effectiveOnDateTime, supplierId, priceListType, activeSupplierEmail, contentBytes, fileName, "Portal", out  insertdId);
        }
        public int GetQueueStatus(int queueId)
        {
            ISupplierPricelistsDataManager dataManager =
                BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return dataManager.GetQueueStatus(queueId);
        }
        public UploadInfo GetUploadInfo(int queueId)
        {
            ISupplierPricelistsDataManager dataManager =
              BEDataManagerFactory.GetDataManager<ISupplierPricelistsDataManager>();
            return dataManager.GetUploadInfo(queueId);
        }
    }

    
}
