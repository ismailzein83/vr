using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
        [Route("GetFilteredSalePriceListCodeChanges")]
        public object GetFilteredSalePriceListCodeChanges(Vanrise.Entities.DataRetrievalInput<SalePriceListChangeQuery> input)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return GetWebResponse(input, manager.GetFilteredPricelistCodeChanges(input));
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
            return salePriceListManager.GenerateSalePriceListFiles(salePriceListInput);
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
            return salePriceListManager.SetCustomerPricelistsAsSent(new List<int> { pricelist.OwnerId }, null);

        }

        [HttpPost]
        [Route("GenerateAndEvaluateSalePriceListEmail")]
        public SalePriceListEvaluatedEmail GenerateAndEvaluateSalePriceListEmail(SalePriceListInput salePriceListInput)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();

            IEnumerable<SalePricelistVRFile> vrFiles = salePriceListManager.GenerateSalePriceListFiles(salePriceListInput);

            var evaluatedObject = salePriceListManager.EvaluateEmail(salePriceListInput.PriceListId, (SalePriceListType)salePriceListInput.PriceListTypeId);
            return new SalePriceListEvaluatedEmail
            {
                SalePricelistVrFiles = vrFiles,
                EvaluatedTemplate = evaluatedObject
            };
        }

    }
}