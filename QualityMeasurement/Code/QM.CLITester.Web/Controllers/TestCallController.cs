using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using QM.CLITester.Business;
using QM.CLITester.Entities;
using Vanrise.Web.Base;

namespace QM.CLITester.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "TestCall")]
   
    public class TestCallController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCountries")]
        public IEnumerable<Country> GetCountries()
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

        [HttpGet]
        [Route("GetTestCall")]
        public TestCallResult GetTestCall(string selectedCountry, string selectedBreakout, string selectedSupplier)
        {
            TestCallManager manager = new TestCallManager();
            return manager.TestCall(selectedCountry, selectedBreakout, selectedSupplier);
        }
    }
}