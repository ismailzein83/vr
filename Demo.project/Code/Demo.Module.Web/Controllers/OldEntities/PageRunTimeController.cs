using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "PageRunTime")]
    [JSONWithTypeAttribute]
    public class PageRunTimeController : BaseAPIController
    {
        PageRunTimeManager pageRunTimeManager = new PageRunTimeManager();

        [HttpPost]
        [Route("GetFilteredPageRunTimes")]
        public object GetFilteredPageRunTimes(DataRetrievalInput<PageRunTimeQuery> input)
        {
            return GetWebResponse(input, pageRunTimeManager.GetFilteredPageRunTimes(input));
        }

        
        [HttpPost]
        [Route("AddPageRunTime")]
        public InsertOperationOutput<PageRunTimeDetails> AddPageRunTime(PageRunTime pageRunTime)
        {
            return pageRunTimeManager.AddPageRunTime(pageRunTime);
        }

        [HttpGet]
        [Route("GetPageRunTimeById")]
        public PageRunTime GetPageRunTimeById(int pageRunTimeId)
        {
            return pageRunTimeManager.GetPageRunTimeById(pageRunTimeId);
        }

        [HttpPost]
        [Route("UpdatePageRunTime")]
        public UpdateOperationOutput<PageRunTimeDetails> UpdatePageRunTime(PageRunTime pageRunTime)
        {
            return pageRunTimeManager.UpdatePageRunTime(pageRunTime);
        }
      
       

    }
}