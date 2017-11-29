using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Entities.SalePricelistChanges;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SalePriceListChange")]
    [JSONWithTypeAttribute]
    public class SalePriceListChangeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSalePriceListRateChanges")]
        public object GetFilteredSalePriceListRateChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredPricelistRateChanges(input));
        }

        [HttpPost]
        [Route("GetFilteredCustomerRatePreviews")]
        public object GetFilteredCustomerRatePreviews(Vanrise.Entities.DataRetrievalInput<CustomerRatePreviewQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredCustomerRatePreviews(input));
        }

         [HttpPost]
         [Route("GetFilteredRoutingProductPreviews")]
        public object GetFilteredRoutingProductPreviews(Vanrise.Entities.DataRetrievalInput<RoutingProductPreviewQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredRoutingProductPreviews(input));
        }
        
        [HttpPost]
        [Route("GetFilteredSalePriceListCodeChanges")]
        public object GetFilteredSalePriceListCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredPricelistCodeChanges(input));
        }
        [HttpPost]
        [Route("GetSalePricelistCodes")]
        public object GetSalePricelistCodes(Vanrise.Entities.DataRetrievalInput<SalePriceListCodeQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredSalePricelistCodes(input));
        }

        [HttpPost]
        [Route("GetFilteredSalePriceListRPChanges")]
        public object GetFilteredSalePriceListRPChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredSalePriceListRPChanges(input));
        }
        [HttpGet]
        [Route("GetOwnerOptions")]
        public SalePriceListOption GetOwnerOptions(int priceListId)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return manager.GetOwnerOptions(priceListId);
        }
        [HttpGet]
        [Route("GetOwnerPriceListType")]
        public int GetOwnerPriceListType(int priceListId)
        {
            var salePriceListManager = new SalePriceListManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);
            return (int)priceList.PriceListType;
        }

        [HttpGet]
        [Route("GetOwnerPricelistTemplateId")]
        public int? GetOwnerPricelistTemplateId(int priceListId)
        {
            var salePriceListManager = new SalePriceListChangeManager();
            return salePriceListManager.GetOwnerPricelistTemplateId(priceListId);
        }

        [HttpGet]
        [Route("GetPricelistSalePricelistVRFile")]
        public IEnumerable<SalePricelistVRFile> GetPricelistSalePricelistVRFile(int salepriceListId, SalePriceListType salePriceListType, int salePriceListTemplateId)
        {
            SalePriceListInput salePriceListInput = new SalePriceListInput
            {
                PriceListId = salepriceListId,
                PriceListTypeId = (int)salePriceListType,
                PricelistTemplateId = salePriceListTemplateId
            };

            SalePriceListManager salePriceListManager = new SalePriceListManager();
            SalePriceListType overriddenListType;
            return salePriceListManager.GenerateSalePriceListFiles(salePriceListInput, out overriddenListType);
        }
        [HttpGet]
        [Route("DownloadSalePriceList")]
        public object DownloadSalePriceList(long fileId)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile vrFile = fileManager.GetFile(fileId);
            return GetExcelResponse(vrFile.Content, vrFile.Name);
        }
        [HttpGet]
        [Route("SetPriceListAsSent")]
        public bool SetPriceListAsSent(int priceListId)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            var pricelist = salePriceListManager.GetPriceList(priceListId);
            return salePriceListManager.SetCustomerPricelistsAsSent(new List<int> { pricelist.OwnerId }, priceListId);

        }

        [HttpPost]
        [Route("GenerateAndEvaluateSalePriceListEmail")]
        public SalePriceListEvaluatedEmail GenerateAndEvaluateSalePriceListEmail(SalePriceListInput salePriceListInput)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            SalePriceListType overriddenListType;

            IEnumerable<SalePricelistVRFile> vrFiles = salePriceListManager.GenerateSalePriceListFiles(salePriceListInput, out overriddenListType);

            var evaluatedObject = salePriceListManager.EvaluateEmail(salePriceListInput.PriceListId, overriddenListType);
            return new SalePriceListEvaluatedEmail
            {
                SalePricelistVrFiles = vrFiles,
                EvaluatedTemplate = evaluatedObject
            };
        }

        [HttpGet]
        [Route("GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId")]
        public SalePriceListEvaluatedEmail GenerateAndEvaluateSalePricelistEmailByPricelistIdAndOwnerId(int pricelistId, int ownerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var customerPriceListTemplateId = carrierAccountManager.GetCustomerPriceListTemplateId(ownerId);
            var customerPriceListType = carrierAccountManager.GetCustomerPriceListType(ownerId);

            SalePriceListInput salePriceListInput = new SalePriceListInput { PriceListId = pricelistId, PricelistTemplateId = customerPriceListTemplateId, PriceListTypeId = (int)customerPriceListType };

            return GenerateAndEvaluateSalePriceListEmail(salePriceListInput);
        }
    }
}