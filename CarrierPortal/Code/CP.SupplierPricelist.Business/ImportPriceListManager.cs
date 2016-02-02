using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Common;
using System.ComponentModel;
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
            var priceListDetail = new PriceListDetail()
            {
                Entity = priceList,
                PriceListStatusDescription = Utilities.GetEnumDescription(priceList.Status),
                PriceListResultDescription = Utilities.GetEnumDescription(priceList.Result),
                PriceListTypeValue = Utilities.GetEnumDescription(priceList.PriceListType),
                UserName = user != null ? user.Name : ""
            };
            return priceListDetail;
        }
        public List<PriceList> GetPriceLists(List<PriceListStatus> listPriceListStatuses)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.GetPriceLists(listPriceListStatuses);
        }

        public bool UpdateInitiatePriceList(long id, int result, int queueId)
        {
            IPriceListDataManager dataManager =
                ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdateInitiatePriceList(id, result, queueId);
        }
        public List<Vanrise.Entities.TemplateConfig> GetUploadPriceListTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SupplierPriceListConnectorInitiateTest);
        }

        public bool UpdatePriceListProgress(long id, int result)
        {
            IPriceListDataManager dataManager =
                   ImportPriceListDataManagerFactory.GetDataManager<IPriceListDataManager>();
            return dataManager.UpdatePriceListProgress(id, result);
            
        }
    }
}
