using CP.SupplierPricelist.Data;
using CP.SupplierPricelist.Entities;
using System.Collections.Generic;
using Vanrise.Common.Business;

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
            priceListUpdateOutputs.PriceLists = dataManager.GetUpdated(ref maxTimeStamp, nbOfRows, Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId());
            priceListUpdateOutputs.MaxTimeStamp = maxTimeStamp;
            return priceListUpdateOutputs;
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

    }
}
