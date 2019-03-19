using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
    {
        public DataManager()
            : base("Initial Catalog=test;Data Source=192.168.25.11;User ID=sa;Password=p@ssw0rd", false)
        {

        }
        internal void TestDiffException()
        {
            //ExecuteNonQueryText("  WAITFOR DELAY '02:00';   ", (cmd) =>
            //{
            //   // cmd.Parameters.Add(new SqlParameter("@Prm", "fgggrt"));
            //});

            ExecuteNonQueryText(" Insert Into Tab1 (Col1) Values (@Prm) ", (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Prm", "fgrt"));
            }, 30);

        }
    }
}
