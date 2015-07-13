using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter : BaseReceiveAdapter
    {
        #region Properties

        public string ConnectionString { get; set; }

        public string Description { get; set; }

        public string Query { get; set; }


        # endregion



        public override void ImportData(Action<IImportedData> receiveData)
        {
            using (var connection = new MySqlConnection(this.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(Query, connection);
                DBReaderImportedData data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                Description = data.Description;
                receiveData(data);
            }

        }

    }
}
