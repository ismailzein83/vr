using Demo.Module.Business;
using Demo.Module.Entities;
using Demo.Module.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Demo_Family")]
    [JSONWithTypeAttribute]
    public class Demo_FamilyController : BaseAPIController
    {
        FamilyManager familyManager = new FamilyManager();
        [HttpPost]
        [Route("GetFilteredFamilies")]
        public object GetFilteredParents(DataRetrievalInput<FamilyQuery> input)
        {
            return GetWebResponse(input, familyManager.GetFilteredFamilies(input));
        }

        [HttpGet]
        [Route("GetFamilyById")]
        public Family GetFamilyById(long familyId)
        {
            return familyManager.GetFamilyById(familyId);
        }

        [HttpPost]
        [Route("UpdateFamily")]
        public UpdateOperationOutput<FamilyDetails> UpdateFamily(Family family)
        {
            return familyManager.UpdateFamily(family);
        }

        [HttpPost]
        [Route("AddFamily")]
        public InsertOperationOutput<FamilyDetails> AddFamily(Family family)
        {
            return familyManager.AddFamily(family);
        }
    }
}