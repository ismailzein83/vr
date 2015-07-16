using System;
using System.Data.SqlClient;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SQLReceiveAdapter
{

    public class SQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {

            DBAdapterArgument dbAdapterArgument = argument as DBAdapterArgument;

            using (var connection = new SqlConnection(dbAdapterArgument.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand(dbAdapterArgument.Query, connection);
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
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();
                connection.Close();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }

    }
}
