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
        public AddTestCallOutput AddNewTestCall(AddTestCallInput testCallInput)
        {
            testCallInput.UserId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();
            testCallInput.ScheduleId = null;
            TestCallManager manager = new TestCallManager();
            return manager.AddNewTestCall(testCallInput);
        }

        [HttpPost]
        [Route("GetUpdated")]
        public LastCallUpdateOutput GetUpdated(GetUpdatedInput input)
        {
            TestCallManager manager = new TestCallManager();
            byte[] maxTimeStamp = input.LastUpdateHandle;
            return manager.GetUpdated(ref maxTimeStamp, input.NbOfRows);
        }


        [HttpPost]
        [Route("GetTotalCallsByUserId")]
        public List<TotalCallsChart> GetTotalCallsByUserId()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetTotalCallsByUserId();
        }


        [HttpPost]
        [Route("GetBeforeId")]
        public List<TestCallDetail> GetBeforeId(GetBeforeIdInput input)
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
        [Route("GetTestTemplates")]
        public IEnumerable<CliTesterConnectorVIConfig> GetTestTemplates()
        {
            TestCallManager manager = new TestCallManager();
            return manager.GetTestTemplates();
        }

        [HttpPost]
        [Route("SendMail")]
        public void SendMail(TestCallInfo input)
        {
            TestCallManager manager = new TestCallManager();
            manager.SendMail(input);
        }

    }
}