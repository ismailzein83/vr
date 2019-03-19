using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApplicationAPI.Controllers
{
    [Route("api/modulePrefix/[controller]/[action]")]
    public class VRBaseController : Controller
    {

    }

    public class MyCustomeAttribute : Attribute
    { }


    public class ValuesController : VRBaseController
    {
        // GET api/values
        [HttpGet]
        [MyCustome]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpGet]
        public Response GetValues(int index)
        {
            return new Response { Text = new string[] { "value1", "value2" }[index] };
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class Response
    {
        public string Text { get; set; }
    }
}
