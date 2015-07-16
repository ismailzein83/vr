using MySql.Data.MySqlClient;
using System;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            DBAdapterArgument dbAdapterArgument = argument as DBAdapterArgument;

            using (var connection = new MySqlConnection(dbAdapterArgument.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(dbAdapterArgument.Query, connection);
                DBReaderImportedData data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                dbAdapterArgument.Description = data.Description;
                receiveData(data);
            }

        }

        public bool IsConnectionAvailable(string connectionString)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                connection.Close();
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

    }
}
