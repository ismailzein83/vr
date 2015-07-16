using MySql.Data.MySqlClient;
using System;
using Vanrise.Integration.Adapters.BaseDB;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter :  DBReceiveAdapter
    {
        public override void ImportData(BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            using (var connection = new MySqlConnection(ConnectionString))
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
