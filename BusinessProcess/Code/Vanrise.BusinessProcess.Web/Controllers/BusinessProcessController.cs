using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    public class BusinessProcessController : ApiController
    {
        [HttpGet]
        public List<BPDefinition> GetDefinitions()
        {
            ProcessManager manager = new ProcessManager();
            return manager.GetDefinitions();
        }

        [HttpGet]
        public List<BPInstance> GetFilteredInstances(int definitionID, string datefrom, string dateto)
        {
            DateTime dateFrom = DateTime.ParseExact(datefrom, "dd/MM/yyyy H:m:s", CultureInfo.CurrentCulture);
            DateTime dateTo = DateTime.ParseExact(dateto, "dd/MM/yyyy H:m:s", CultureInfo.CurrentCulture);          
            ProcessManager manager = new ProcessManager();
            return manager.GetFilteredInstances( definitionID,  dateFrom,  dateTo);
        }

        [HttpGet]
        public List<BPTrackingMessage> GetTrackingsByInstanceId(long ProcessInstanceID)
        {
            ProcessManager manager = new ProcessManager();
            return manager.GetTrackingsByInstanceId(ProcessInstanceID);
        }
         
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "value3", "value4" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}