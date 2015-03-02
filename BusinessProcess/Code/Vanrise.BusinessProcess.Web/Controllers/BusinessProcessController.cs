using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Web.ModelMappers;
using Vanrise.BusinessProcess.Web.Models;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    public class BusinessProcessController : ApiController
    {
        [HttpGet]
        public List<BPDefinition> GetDefinitions()
        {
            BPClient manager = new BPClient();
            return manager.GetDefinitions();
        }

        [HttpGet]
        public List<BPInstanceModel> GetFilteredInstances(int definitionID, string datefrom, string dateto)
        {
            DateTime dateFrom = DateTime.ParseExact(datefrom, "dd/MM/yyyy H:m:s", CultureInfo.CurrentCulture);
            DateTime dateTo = (String.IsNullOrEmpty(dateto)) ? DateTime.Now : DateTime.ParseExact(dateto, "dd/MM/yyyy H:m:s", CultureInfo.CurrentCulture);
            BPClient manager = new BPClient();
            return BPMappers.MapTMapInstances(manager.GetFilteredInstances(definitionID, dateFrom, dateTo));

        }

        [HttpGet]
        public List<BPTrackingMessageModel> GetTrackingsByInstanceId(long ProcessInstanceID)
        {
            BPClient manager = new BPClient();
            return   BPMappers.MapTrackingMessages( manager.GetTrackingsByInstanceId(ProcessInstanceID) );
        }
        public List<EnumModel> GetStatusList()
        {
            var lst  = new List<EnumModel>();
            foreach(var val in Enum.GetValues(typeof(BPInstanceStatus)))
            {
                EnumModel item = new EnumModel{
                    Value = (int)val ,
                    Description = ((BPInstanceStatus)val).ToString()
                };
                lst.Add(item);
            }
            return lst;
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