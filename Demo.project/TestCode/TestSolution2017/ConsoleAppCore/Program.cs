using StandartLib;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Threading.Tasks;
using Vanrise.Common;

namespace ConsoleAppCore
{
    class Program
    {
        [ThreadStatic]
        static int? threadVar;


        static void Main(string[] args)
        {
            string connectionString = new EntityConnectionStringBuilder

            {

                Metadata = "res://*",

                Provider = "System.Data.SqlClient",

                ProviderConnectionString = new System.Data.SqlClient.SqlConnectionStringBuilder

                {

                    InitialCatalog = "Retail_Dev_Transaction",

                    DataSource = ".",

                    IntegratedSecurity = false,

                    UserID = "sa",                 // User ID such as "sa"

                    Password = "p@ssw0rd",               // hide the password

                }.ConnectionString

            }.ConnectionString;
            using (EntityConnection conn = new EntityConnection(connectionString))
            {
                //conn.Open();
                EntityCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"Select t.MyValue From MyEntities.MyTable As t";
                var cmdTree = cmd.CommandText;
                conn.Close();
            }
            //Console.WriteLine("Hello World!");
            //Class1 class1 = new Class1();
            //class1.Test();

            //StandartLib.CSharpCompilationOutput output;
            //Class1.TryCompileClass("fddsf", "public class TestClassss {}", out output);

            Parallel.For(0, 5, (i) =>
            {                
                System.Threading.Thread.Sleep(i * 1000);
                Console.WriteLine($"{DateTime.Now}: Before Set: threadVar is {threadVar}");
                threadVar = i;
                Console.WriteLine($"{DateTime.Now}: After Set: threadVar is {threadVar}");
            });
            Console.ReadKey();
            //var serialized = Serializer.Serialize("dsfgdsgdsg");
        }
    }


}
