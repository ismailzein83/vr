using Aspose.Cells;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "ReceivedSupplierPriceList")]
    public class WhS_ReceivedSupplierPriceListController : BaseAPIController
    {
        //[HttpPost]
        //[Route("GetFilteredReceivedSupplierPriceList")]
        //public object GetFilteredReceivedSupplierPriceList(Vanrise.Entities.DataRetrievalInput<ReceivedSupplierPriceListQuery> input)
        //{
        //    ReceivedSupplierPriceListManager manager = new ReceivedSupplierPriceListManager();
        //    return GetWebResponse(input, manager.GetFilteredReceivedSupplierPriceList(input));
        //}
    }
}