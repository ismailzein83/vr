using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Common;
using System.Linq;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace CP.SupplierPricelist.Business
{
    public class ImportPriceListManager
    {
        public bool Insert(PriceList priceList)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            priceList.UserId = SecurityContext.Current.GetLoggedInUserId();
            dataManager.Insert(priceList);

            return true;
        }

        public PriceListlUpdateOutput GetUpdated(ref byte[] maxTimeStamp, int nbOfRows)
        {
            PriceListlUpdateOutput priceListUpdateOutputs = new PriceListlUpdateOutput();
            IPriceListDataManager dataManager =
             ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            List<PriceList> listPriceLists = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, SecurityContext.Current.GetLoggedInUserId());
            List<PriceListDetail> listPriceListDetails = listPriceLists.Select(priceListDetailMapper).ToList();
            priceListUpdateOutputs.ListPriceListDetails = listPriceListDetails;
            priceListUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return priceListUpdateOutputs;
        }

        PriceListDetail priceListDetailMapper(PriceList priceList)
        {
            UserManager userManager = new UserManager();
            User user = userManager.GetUserbyId(priceList.UserId);

            CustomerManager customerManager = new CustomerManager();
            var priceListDetail = new PriceListDetail()
            {
                Entity = priceList,
                PriceListStatusDescription = Utilities.GetEnumDescription(priceList.Status),
                PriceListResultDescription = Utilities.GetEnumDescription(priceList.Result),
                PriceListTypeValue = Utilities.GetEnumDescription(priceList.PriceListType),
                UserName = user != null ? user.Name : ""
            };
            Customer customer = customerManager.GetCustomer(priceList.CustomerId);
            priceListDetail.CustomerName = customer != null ? customer.Name : "";

            var customerSuppliers = new CustomerManager().GetCustomerSuppliers(priceList.CustomerId);
            var accountInfo = customerSuppliers.SingleOrDefault(a => a.SupplierId.Equals(priceList.CarrierAccountId));
            priceListDetail.CarrierAccountName = accountInfo != null ? accountInfo.SupplierName : "";
            return priceListDetail;
        }
        public List<PriceList> GetPriceLists(List<PriceListStatus> listPriceListStatuses)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.GetPriceLists(listPriceListStatuses);
        }

        public bool UpdatePriceListUpload(long id, int result, object uploadInformation, int uploadRetryCount)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdatePriceListUpload(id, result, uploadInformation, uploadRetryCount);
        }

        public List<Vanrise.Entities.TemplateConfig> GetConnectorTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CustomerConnector);
        }

        public bool UpdatePriceListProgress(long id, int status, int result, int resultRetryCount, string alertMessage)
        {
            IPriceListDataManager dataManager =
                   ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdatePriceListProgress(id, status, result, resultRetryCount, alertMessage);

        }
    }
}
