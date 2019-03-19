using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DotNetWebApplicationMVCAndAPI.Controllers
{
    [RoutePrefix("api/ModName/myapi")]
    public class MyAPIController : ApiController
    {
        [Route("GetItemFromArray")]
        public Response GetItemFromArray(int itmIndex)
        {
            return new Response { Text = new string[] { "Item 1", "Item 2", "Item 3" }[itmIndex] };
        }
    }

    public class Response
    {
        public string Text { get; set; }
    }
}
