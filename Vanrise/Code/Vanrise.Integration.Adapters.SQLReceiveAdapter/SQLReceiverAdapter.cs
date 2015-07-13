using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Vanrise.Integration.Adapters.SQLReceiverAdapter
{

    public class ConnectionString 
    {
        public string DataSource { get; set; } //"(local)"

        public bool IntegratedSecurity { get; set; }

        public string InitialCatalog { get; set; }


    }

    public class SQLReceiveAdapter : BaseReceiveAdapter
    {
        #region Properties

        public ConnectionString ConnectionString { get; set; }

        public string Description { get; set; }

        public string Query { get; set; }
       

        # endregion



        public override void ImportData(Action<IImportedData> receiveData)
        {

            SqlConnectionStringBuilder builder =  new SqlConnectionStringBuilder();
            builder["Data Source"] = ConnectionString.DataSource;
            builder["integrated Security"] = ConnectionString.IntegratedSecurity;
            builder["Initial Catalog"] = ConnectionString.InitialCatalog;



            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(Query, connection);
                DBReaderImportedData data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                Description = data.Description;
                receiveData(data);
            }

        }

    }
}
