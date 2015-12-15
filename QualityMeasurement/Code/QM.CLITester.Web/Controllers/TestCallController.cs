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
        [HttpGet]
        [Route("GetCountries")]
        public IEnumerable<CLITester.Entities.Country2> GetCountries()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetCachedCountries();
        }

        [HttpGet]
        [Route("GetBreakouts")]
        public IEnumerable<Breakout> GetBreakouts(string selectedCountry)
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetCachedBreakouts(selectedCountry);
        }

        [HttpPost]
        [Route("AddNewTestCall")]
        public InsertOperationOutput<TestCallQueryInsert> AddNewTestCall(TestCallQueryInsert testCallResult)
        {
            TestCallManager manager = new TestCallManager();
            return manager.AddNewTestCall(testCallResult);
        }

        [HttpPost]  
        [Route("GetUpdatedTestCalls")]
        public LastCallUpdateOutput GetUpdatedTestCalls(LastCallUpdateInput input)
        {
            TestCallManager manager = new TestCallManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;    
            return manager.GetUpdatedTestCalls(ref maxTimeStamp);
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