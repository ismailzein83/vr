using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using WebApplication2.Controllers;

namespace WebApplication2
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            //throw new Exception("Exception inside method");
            return "Hello World";
        }

        [WebMethod]
        public TestMethodOutput TestMethod2(TestMethodInput input, TestMethodInput input2)
        {
            //throw new Exception("Exceptions thrown to see response");
            return new TestMethodOutput { Text1 = "Test Method2: " + input.Text, Text2 = ". Input Value 2: " + (input2 != null ? input2.Text : "") };
        }

        [WebMethod]
        public MyTestMethodOutput TestMethod(MyTestMethodInput input1)
        {
            //throw new Exception("Exception inside method");
            return new MyTestMethodOutput
            {
                Text1 = "this is the Output Value1: " + (input1 != null ? input1.Text : ""),
                List = new List<string> { "Item1", "Item 2", "Item 3" },
                ListOfObjects = new List<MyTestMethodOutput> { new MyTestMethodOutput { Text1 = "Item 1 Text 1" }, new MyTestMethodOutput { Text1 = "Item 2 Text 1" }, new MyTestMethodOutput { Text1 = "Item 3 Text 1" } }
            };
        }
    }

   // [Serializable]
    //[System.Runtime.Serialization.DataContract]
    public class TestMethodInput
    {
        //[System.Runtime.Serialization.DataMember(Name = "Text")]
        public string Text { get; set; }
    }

    //[Serializable]
    //[System.Runtime.Serialization.DataContract]
    public class TestMethodOutput
    {
        //[System.Runtime.Serialization.DataMember]
        public string Text1 { get; set; }

        //[System.Runtime.Serialization.DataMember]
        public string Text2 { get; set; }
    }
}
