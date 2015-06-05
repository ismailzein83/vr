using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<CDR> GetCDRData(GetCDRSummaryInput input)
        {
         
            return __cdrManager.GetCDRData(input.Filter,input.From, input.To, input.Size, input.CDROption);
        }
    }
    #region Argument Classes
    public class GetCDRSummaryInput
    {
        public CDRFilter Filter { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Size { get; set; }
        public string CDROption { get; set; }
    }
    #endregion
}