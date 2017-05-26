using System.IO;
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
        [Route("GetOwnerName")]
        public string GetOwnerName(int priceListId)
        {
            SalePriceListChangeManager manager = new SalePriceListChangeManager();
            return manager.GetOwnerName(priceListId);
        }
        [HttpGet]
        [Route("GetOwnerPriceListType")]
        public int GetOwnerPriceListType(int priceListId)
        {
            var salePriceListManager = new SalePriceListManager();
            var priceList = salePriceListManager.GetPriceList(priceListId);

            var carrierAccountManager = new CarrierAccountManager();
            return (int)carrierAccountManager.GetPriceListType(priceList.OwnerId);
        }

        [HttpGet]
        [Route("DownloadSalePriceList")]
        public object DownloadSalePriceList(long salepriceListId, SalePriceListType salePriceListType)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            VRFile file = salePriceListManager.DownloadSalePriceList(salepriceListId, salePriceListType);
            return GetExcelResponse(file.Content, file.Name);
        }

        [HttpGet]
        [Route("GenerateSalePriceListFile")]
        public long GenerateSalePriceListFile(long salepriceListId, SalePriceListType salePriceListType)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            VRFile file = salePriceListManager.DownloadSalePriceList(salepriceListId, salePriceListType);
            VRFileManager fileManager = new VRFileManager();
            return fileManager.AddFile(file);
        }

        [HttpGet]
        [Route("EvaluateSalePriceListEmail")]
        public VRMailEvaluatedTemplate EvaluateSalePriceListEmail(long salepriceListId)
        {
            SalePriceListManager salePriceListManager = new SalePriceListManager();
            return salePriceListManager.EvaluateEmail(salepriceListId);
        }
    }
}