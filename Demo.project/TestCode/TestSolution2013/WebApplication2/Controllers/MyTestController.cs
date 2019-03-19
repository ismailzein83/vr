using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication2.Controllers
{
    public class MyTestController : ApiController
    {
        [HttpGet]
        public string TestCall(string input)
        {
            //throw new Exception("Exception inside method");
            return string.Concat(input, ":43654 ", DateTime.Now);
        }

        [HttpGet]
        public MyTestMethodOutput TestGetObject(string input)
        {
            //throw new Exception("Exception inside method");
            return new MyTestMethodOutput
            {
                Text1 = "this is the Output Value1: " + input,
                List = new List<string> { "Item1", "Item 2", "Item 3" },
                ListOfObjects = new List<MyTestMethodOutput> { new MyTestMethodOutput { Text1 = "Item 1 Text 1" }, new MyTestMethodOutput { Text1 = "Item 2 Text 1" }, new MyTestMethodOutput { Text1 = "Item 3 Text 1" } }
            };
        }

        [HttpPost]
        public MyTestMethodOutput TestMethod(MyTestMethodInput input1)
        {
            throw new Exception("Exception inside method");
            return new MyTestMethodOutput
            {
                Text1 = "this is the Output Value1: " + (input1 != null ? input1.Text : ""),
                List = new List<string> {  "Item1", "Item 2", "Item 3"},
                ListOfObjects = new List<MyTestMethodOutput> { new MyTestMethodOutput { Text1 = "Item 1 Text 1" }, new MyTestMethodOutput { Text1 = "Item 2 Text 1" }, new MyTestMethodOutput { Text1 = "Item 3 Text 1" } }
            };
        }
    }

    public class MyTestMethodInput
    {
        public string Text { get; set; }
    }

    public class MyTestMethodOutput
    {
        public string Text1 { get; set; }

        public string Text2 { get; set; }

        public List<string> List { get; set; }

        public List<MyTestMethodOutput> ListOfObjects { get; set; }
    }
}
