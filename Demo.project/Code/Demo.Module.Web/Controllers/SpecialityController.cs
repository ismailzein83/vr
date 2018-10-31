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
  [RoutePrefix(Constants.ROUTE_PREFIX+"Speciality")]
    [JSONWithTypeAttribute]
    public class SpecialityController:BaseAPIController{

      SpecialityManager specialityManager = new SpecialityManager();


        [HttpGet]
        [Route("GetSpecialitiesInfo")]
        public IEnumerable<SpecialityInfo> SpecialitiesInfo(string filter = null)
        {
            return specialityManager.GetSpecialitiesInfo();
        }

      [HttpGet]
      [Route("GetSpecialityById")]
      public Speciality GetSpecialityById(int specialityId)
      {
          return specialityManager.GetSpecialityById(specialityId);

      }

     }
}
