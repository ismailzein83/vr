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
        public CDRBigResult GetCDRData(GetCDRSummaryInput input)
        {
            System.Threading.Thread.Sleep(1000);
            return __cdrManager.GetCDRData(input.TempTableKey, input.Filter, input.From, input.To, input.FromRow, input.ToRow, input.Size, input.CDROption, input.OrderBy, input.IsDescending);
        }
        [HttpPost]
        public HttpResponseMessage ExportCDRData(GetCDRSummaryInput CDRLogSummaryInput)
        {
            System.Threading.Thread.Sleep(1000);
            CDRBigResult records = __cdrManager.GetCDRData(CDRLogSummaryInput.TempTableKey, CDRLogSummaryInput.Filter, CDRLogSummaryInput.From, CDRLogSummaryInput.To, 0, CDRLogSummaryInput.Size, CDRLogSummaryInput.Size, CDRLogSummaryInput.CDROption, CDRLogSummaryInput.OrderBy, CDRLogSummaryInput.IsDescending);
            return __cdrManager.ExportCDRData(records);
        }
    }
    
    #region Argument Classes
    public class GetCDRSummaryInput
    {
       public string TempTableKey { get; set; }

       public int FromRow { get; set; }
       public int ToRow { get; set; }
       public BillingCDRMeasures OrderBy { get; set; }
       public bool IsDescending { get; set; }
        
        public CDRFilter Filter { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Size { get; set; }
        public BillingCDROptionMeasures CDROption { get; set; }
    }
    #endregion
}