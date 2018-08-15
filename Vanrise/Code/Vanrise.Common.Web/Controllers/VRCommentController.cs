using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRComment")]
    public class VRCommentController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredVRComments")]
        public object GetFilteredVRComments(Vanrise.Entities.DataRetrievalInput<VRCommentQuery> input)
        {
            VRCommentManager manager = new VRCommentManager();
            return GetWebResponse(input, manager.GetFilteredVRComments(input));
        }

        [HttpPost]
        [Route("AddVRComment")]
        public Vanrise.Entities.InsertOperationOutput<VRCommentDetail> AddVRComment(VRComment vRComment)
        {
            VRCommentManager manager = new VRCommentManager();
            return manager.AddVRComment(vRComment);
        }
       
    }
}