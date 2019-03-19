
using System;
using System.Data.SqlClient;

namespace StandardOnlyLib
{
    
     public class StandardOnlySqlClass 
    {
        public void TestSQL()
        {
            //System.IO.File.ReadAllBytes("fff");
            using (var conn = new SqlConnection(@"Data Source=.;Initial Catalog=TOneWFTracking;User ID=sa; Password=p@ssw0rd"))
            {
                conn.Open();

                conn.Close();
            }
            
           // Microsoft.AspNetCore.Http.HttpContext.CU
        }

       // [Route("")]
        public void TestDynamic()
        {
            dynamic d = new TestClass();
            d.Prop = "3434";
        }

        private class TestClass
        {
            public string Prop { get; set; }
        }


        //public void TestCompile()
        //{
        //    System.CodeDom.Compiler.IndentedTextWriter textWriter = new System.CodeDom.Compiler.IndentedTextWriter(null);
        //}
    }
}
