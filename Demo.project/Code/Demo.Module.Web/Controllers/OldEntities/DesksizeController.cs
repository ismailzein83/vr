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
  [RoutePrefix(Constants.ROUTE_PREFIX+"Desksize")]
    [JSONWithTypeAttribute]
     public class DesksizeController:BaseAPIController
     {
      DesksizeManager desksizeManager = new DesksizeManager();
      [HttpGet]
      [Route("GetDesksizesInfo")]
      public IEnumerable<DesksizeInfo> DesksizeInfo(string filter=null)
      {
          return desksizeManager.GetDesksizesInfo(); 

      }
      [HttpGet]
      [Route("GetDesksizeById")]
      public Desksize GetDesksizeById(int desksizeId)
      {
          return desksizeManager.GetDesksizeById(desksizeId);

      }

     }
}