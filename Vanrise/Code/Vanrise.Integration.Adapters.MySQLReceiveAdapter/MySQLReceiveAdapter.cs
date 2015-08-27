using MySql.Data.MySqlClient;
using System;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Func<IImportedData, bool> receiveData)
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
                    data.Reader = command.ExecuteReader();

                    if (receiveData(data))
                    {
                        this.UpdateLastImportedId(dataSourceId, data, dbAdapterState);
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

        private void UpdateLastImportedId(int dataSourceId, DBReaderImportedData data, DBAdapterState dbAdapterState)
        {
            Vanrise.Integration.Business.DataSourceManager manager = new Business.DataSourceManager();
            dbAdapterState.LastImportedId = data.LastImportedId;
            manager.UpdateAdapterState(dataSourceId, dbAdapterState);
        }

    }
}
