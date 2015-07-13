using System;
using System.Data.SqlClient;
using Vanrise.Integration.Adapters.BaseDB;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SQLReceiveAdapter
{

    public class SQLReceiveAdapter :  DBReceiveAdapter
    {
        public override void ImportData(Action<IImportedData> receiveData)
        {
            using (var connection = new SqlConnection(this.ConnectionString))
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
