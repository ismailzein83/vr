using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WebApplication2
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string DoWork()
        {
            return "Response from DoWOrk";
        }
        public TestMethodOutput TestMethod2(TestMethodInput input, TestMethodInput input2)
        {
            //throw new Exception("Exceptions thrown to see response");
            return new TestMethodOutput { Text1 = "Test Method2: " + input.Text, Text2 = ". Input Value 2: " + (input2 != null ? input2.Text : "") };
        }

    }

   
}
