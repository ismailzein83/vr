using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "University")]
    public class UniversityController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredUniversities")]
        public object GetFilteredUniversities(DataRetrievalInput<UniversityQuery> input)
        {
            UniversityManager universityManager = new UniversityManager();
            return GetWebResponse(input, universityManager.GetFilteredUniversities(input));
        }

        [HttpGet]
        [Route("GetUniversitiesInfo")]
        public IEnumerable<UniversityInfo> GetUniversitiesInfo()
        {
            UniversityManager universityManager = new UniversityManager();
            return universityManager.GetUniversitiesInfo();
        }

        [HttpGet]
        [Route("GetUniversityById")]
        public University GetUniversityById(int universityId)
        {
            UniversityManager universityManager = new UniversityManager();
            return universityManager.GetUniversityById(universityId);
        }

        [HttpPost]
        [Route("AddUniversity")]
        public InsertOperationOutput<UniversityDetails> AddUniversity(University university)
        {
            UniversityManager universityManager = new UniversityManager();
            return universityManager.AddUniversity(university);
        }

        [HttpPost]
        [Route("UpdateUniversity")]
        public UpdateOperationOutput<UniversityDetails> UpdateUniversity(University university)
        {
            UniversityManager universityManager = new UniversityManager();
            return universityManager.UpdateUniversity(university);
        }

        [HttpGet]
        [Route("DeleteUniversity")]
        public DeleteOperationOutput<UniversityDetails> DeleteUniversity(int universityId)
        {
            UniversityManager universityManager = new UniversityManager();
            return universityManager.Delete(universityId);
        }
    }
}