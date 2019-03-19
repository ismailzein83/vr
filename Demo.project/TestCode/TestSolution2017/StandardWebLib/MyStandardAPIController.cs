using System;
//using System.Collections.Generic;
//using System.Text;
using System.Web.Http;

namespace StandardWebLib
{
    //[Microsoft.AspNetCore.Mvc.Route("api/ModName/MyStandardAPI")]
    [RoutePrefix("api/ModName/MyStandardAPI")]
    //[GeneratedController("api/ModName/MyStandardAPI/[action]")]
    public class MyStandardAPIController : CommonWebLib.BaseAPIController
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
