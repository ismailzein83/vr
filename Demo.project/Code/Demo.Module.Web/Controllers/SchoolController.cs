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
    [RoutePrefix(Constants.ROUTE_PREFIX + "School")]
    [JSONWithTypeAttribute]
    public class SchoolController : BaseAPIController
    {
        SchoolManager schoolManager = new SchoolManager();
        [HttpPost]
        [Route("GetFilteredSchools")]
        public object GetFilteredSchools(DataRetrievalInput<SchoolQuery> input)
        {
            return GetWebResponse(input, schoolManager.GetFilteredSchools(input));
        }

        [HttpGet]
        [Route("GetSchoolById")]
        public School GetSchoolById(int schoolId)
        {
            return schoolManager.GetSchoolById(schoolId);
        }

        [HttpPost]
        [Route("UpdateSchool")]
        public UpdateOperationOutput<SchoolDetails> UpdateSchool(School school)
        {
            return schoolManager.UpdateSchool(school);
        }

        [HttpPost]
        [Route("AddSchool")]
        public InsertOperationOutput<SchoolDetails> AddSchool(School school)
        {
            return schoolManager.AddSchool(school);
        }

        [HttpGet]
        [Route("GetSchoolsInfo")]
        public IEnumerable<SchoolInfo> GetSchoolsInfo(string filter = null)
        {
            SchoolInfoFilter schoolInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<SchoolInfoFilter>(filter) : null;
            return schoolManager.GetSchoolsInfo(schoolInfoFilter);
        }
        

    }
}