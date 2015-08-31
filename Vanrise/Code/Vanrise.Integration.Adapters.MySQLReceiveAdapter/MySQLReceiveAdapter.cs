using MySql.Data.MySqlClient;
using System;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            DBAdapterArgument dbAdapterArgument = argument as DBAdapterArgument;
            DBAdapterState dbAdapterState = adapterState as DBAdapterState;

            DBReaderImportedData data = null;

            try
            {

                dbAdapterArgument.Query = dbAdapterArgument.Query.ToLower().Replace("{lastimportedid}", dbAdapterState.LastImportedId);

                using (var connection = new MySqlConnection(dbAdapterArgument.ConnectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(dbAdapterArgument.Query, connection);
                    data = new DBReaderImportedData();
                    while ((data.Reader = command.ExecuteReader()).HasRows)
                    {
                        data.LastImportedId = dbAdapterState.LastImportedId;
                        
                        receiveData(data);

                        dbAdapterState.LastImportedId = data.LastImportedId;
                        base.UpdateAdapterState(dataSourceId, dbAdapterState);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("An error occurred in My SQL Adapter while importing data. Exception Details: {0}", ex.Message);
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
