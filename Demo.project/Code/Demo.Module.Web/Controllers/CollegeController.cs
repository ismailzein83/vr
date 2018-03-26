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
    [RoutePrefix(Constants.ROUTE_PREFIX + "College")]
      [JSONWithType]
    public class CollegeController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredColleges")]
        public object GetFilteredColleges(DataRetrievalInput<CollegeQuery> input)
        {
            CollegeManager collegeManager = new CollegeManager();
            return GetWebResponse(input, collegeManager.GetFilteredColleges(input));
        }

        [HttpGet]
        [Route("GetCollegesInfo")]
        public IEnumerable<CollegeInfo> GetCollegesInfo()
        {
            CollegeManager collegeManager = new CollegeManager();
            return collegeManager.GetCollegesInfo();
        }

        [HttpGet]
        [Route("GetCollegeById")]
        public College GetCollegeById(int collegeId)
        {
            CollegeManager collegeManager = new CollegeManager();
            return collegeManager.GetCollegeById(collegeId);
        }

        [HttpPost]
        [Route("AddCollege")]
        public InsertOperationOutput<CollegeDetails> AddCollege(College college)
        {
            CollegeManager collegeManager = new CollegeManager();
            return collegeManager.AddCollege(college);
        }

        [HttpPost]
        [Route("UpdateCollege")]
        public UpdateOperationOutput<CollegeDetails> UpdateCollege(College college)
        {
            CollegeManager collegeManager = new CollegeManager();
            return collegeManager.UpdateCollege(college);
        }

        [HttpGet]
        [Route("DeleteCollege")]
        public DeleteOperationOutput<CollegeDetails> DeleteCollege(int collegeId)
        {
            CollegeManager collegeManager = new CollegeManager();
            return collegeManager.Delete(collegeId);
        }
    }
}