using System;
using System.Web.Http;
using Vanrise.Web.Base;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerTargetMatch")]
    public class CustomerTargetMatchController : BaseAPIController
    {
        [HttpGet]
        [Route("DownloadCustomerTargetMatchTemplate")]
        public object DownloadCustomerTargetMatchTemplate(int customerId)
        {
            byte[] fileContent = new CustomerTargetMatchManager().GetCustomerTargetMatchTemplateFileContent(customerId);
            return GetExcelResponse(fileContent, "CustomerTargetMatch.xlsx");
        }
    }
}