using System;
using System.Data.SqlClient;

namespace StandardLib3
{
    public class Class1
    {
        public void TestSQL()
        {
            using (var conn = new SqlConnection(@"Data Source=.;Initial Catalog=TOneWFTracking;User ID=sa; Password=p@ssw0rd"))
            {
                conn.Open();

                conn.Close();
            }
        }
    }
}
