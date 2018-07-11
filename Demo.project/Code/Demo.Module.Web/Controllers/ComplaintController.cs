using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Complaint")]
    [JSONWithTypeAttribute]
    public class ComplaintController : BaseAPIController
    {
        ComplaintManager complaintManager = new ComplaintManager();

        [HttpGet]
        [Route("GetComplaints")]
        public List<Complaint> GetComplaints()
        {
            return complaintManager.GetComplaints();

        }
       
    }
}
