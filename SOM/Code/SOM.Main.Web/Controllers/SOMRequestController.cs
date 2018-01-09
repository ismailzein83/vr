using SOM.Main.Business;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SOMRequest")]
    public class SOMRequestController : BaseAPIController
    {
        static SOMRequestManager s_manager = new SOMRequestManager();

        [HttpPost]
        [Route("CreateSOMRequest")]
        public CreateSOMRequestOutput CreateSOMRequest(CreateSOMRequestInput input)
        {
            return s_manager.CreateSOMRequest(input);
        }

        [HttpPost]
        [Route("GetFilteredSOMRequests")]
        public object GetFilteredSOMRequests(Vanrise.Entities.DataRetrievalInput<SOMRequestQuery> input)
        {
            return GetWebResponse(input, s_manager.GetFilteredSOMRequests(input)); 
        }
    }
}