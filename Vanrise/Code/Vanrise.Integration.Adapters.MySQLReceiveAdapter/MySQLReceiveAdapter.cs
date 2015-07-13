using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Adapters.BaseDB;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter :  DBReceiveAdapter
    {
      
        public override void ImportData(Action<IImportedData> receiveData)
        {
            using (var connection = new MySqlConnection(base.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(base.Query, connection);
                DBReaderImportedData data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                Description = data.Description;
                receiveData(data);
            }

        }

    }
}
