using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.LCR.Data;
using TOne.LCR.Entities;

namespace TOne.Web.Online.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public List<SupplierCodeInfo> GetActiveSupplierCodeInfo()
        {
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            return dataManager.GetActiveSupplierCodeInfo(DateTime.Today, DateTime.Now);
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
