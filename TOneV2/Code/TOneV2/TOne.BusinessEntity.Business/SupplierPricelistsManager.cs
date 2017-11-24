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

    public class PriceListFileSettings : Vanrise.Entities.VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7042919C-A79D-4600-943F-A0BE8E3CC4F7"); }
        }

        public int PriceListId { get; set; }

        Vanrise.Security.Business.SecurityManager s_securityManager = new Vanrise.Security.Business.SecurityManager();
        public override bool DoesUserHaveViewAccess(Vanrise.Entities.IVRFileDoesUserHaveViewAccessContext context)
        {
            return s_securityManager.HasPermissionToActions("WhS_BE/SupplierPricelist/GetFilteredSupplierPricelist", context.UserId);
        }

    }
}
