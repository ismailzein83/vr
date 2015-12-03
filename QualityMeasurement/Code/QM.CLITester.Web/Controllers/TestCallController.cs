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
        public IEnumerable<CLITester.Entities.Country> GetCountries()
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

        [HttpGet]
        [Route("GetSuppliers")]
        public IEnumerable<Supplier> GetSuppliers()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetCachedSuppliers();
        }

        [HttpPost]
        [Route("AddNewTestCall")]
        public InsertOperationOutput<TestCallResult> AddNewTestCall(TestCallResult testCallResult)
        {
            TestCallManager manager = new TestCallManager();
            return manager.AddNewTestCall(testCallResult);
        }

        [HttpPost]
        [Route("GetFilteredTestCalls")]
        public object GetFilteredTestCalls(Vanrise.Entities.DataRetrievalInput<TestCallResultQuery> input)
        {
            TestCallManager manager = new TestCallManager();
            return GetWebResponse(input, manager.GetFilteredTestCalls(input));
        }
    }
}