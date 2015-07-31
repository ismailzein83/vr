using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
namespace TOne.Analytics.Web.Controllers
{
    public class CDRController : Vanrise.Web.Base.BaseAPIController
    {
        private readonly CDRManager __cdrManager;
        public CDRController()
        {

            __cdrManager = new CDRManager();
        }
        [HttpPost]
        public object GetCDRData(Vanrise.Entities.DataRetrievalInput<CDRSummaryInput> input)
        {
            System.Threading.Thread.Sleep(1000);
             return GetWebResponse(input,__cdrManager.GetCDRData(input));
          
        }
        //[HttpPost]
        //public HttpResponseMessage ExportCDRData(CDRSummaryInput input)
        //{
        //    System.Threading.Thread.Sleep(1000);
        //    CDRBigResult records = __cdrManager.GetCDRData(input.TempTableKey, input.Filter, input.From, input.To, 0, input.Size, input.Size, input.CDROption, input.OrderBy, input.IsDescending);
        //    return __cdrManager.ExportCDRData(records);
        //}
    }
    

}