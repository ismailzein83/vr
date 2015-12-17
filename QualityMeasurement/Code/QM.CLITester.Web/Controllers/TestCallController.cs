using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using QM.CLITester.Business;
using QM.CLITester.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace QM.CLITester.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "TestCall")]
   
    public class TestCallController : BaseAPIController
    {

        [HttpPost]
        [Route("AddNewTestCall")]
        public InsertOperationOutput<TestCallQueryInsert> AddNewTestCall(TestCallQueryInsert testCallResult)
        {
            TestCallManager manager = new TestCallManager();
            return manager.AddNewTestCall(testCallResult);
        }

        [HttpPost]
        [Route("GetUpdated")]
        public LastCallUpdateOutput GetUpdated(LastCallUpdateInput input)
        {
            TestCallManager manager = new TestCallManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetUpdated(ref maxTimeStamp, input.NbOfRows);
        }

        [HttpPost]
        [Route("GetBeforeId")]
        public LastCallUpdateOutput GetBeforeId(LastCallUpdateInput input)
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetBeforeId(input);
        }

        [HttpPost]
        [Route("GetFilteredTestCalls")]
        public object GetFilteredTestCalls(Vanrise.Entities.DataRetrievalInput<TestCallQuery> input)
        {
            TestCallManager manager = new TestCallManager();
            return GetWebResponse(input, manager.GetFilteredTestCalls(input));
        }

        [HttpGet]
        [Route("GetInitiateTestTemplates")]
        public List<TemplateConfig> GetInitiateTestTemplates()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetInitiateTestTemplates();
        }

        [HttpGet]
        [Route("GetTestProgressTemplates")]
        public List<TemplateConfig> GetTestProgressTemplates()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetTestProgressTemplates();
        }
    }
}