using MySql.Data.MySqlClient;
using System;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.MSQLReceiveAdapter
{

    public class MySQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(int dataSourceId, BaseAdapterArgument argument, Func<IImportedData, bool> receiveData)
        {
            DBAdapterArgument dbAdapterArgument = argument as DBAdapterArgument;

            dbAdapterArgument.Query = dbAdapterArgument.Query.ToLower().Replace("{startindex}", dbAdapterArgument.StartIndex);

            using (var connection = new MySqlConnection(dbAdapterArgument.ConnectionString))
            {
                connection.Open();
                var command = new MySqlCommand(dbAdapterArgument.Query, connection);
                DBReaderImportedData data = new DBReaderImportedData();
                data.Reader = command.ExecuteReader();
                data.StartIndex = dbAdapterArgument.StartIndex;
                //dbAdapterArgument.Description = data.Description;
                if (receiveData(data))
                {
                    this.UpdateStartIndex(dataSourceId, data);
                }
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

        private void UpdateStartIndex(int dataSourceId, DBReaderImportedData data)
        {
            Vanrise.Integration.Business.DataSourceManager manager = new Business.DataSourceManager();
            DataSourceSettings settings = manager.GetDataSourceSettings(dataSourceId);
            ((DBAdapterArgument)settings.AdapterArgument).StartIndex = data.StartIndex;
            manager.UpdateDataSourceSettings(dataSourceId, settings);
        }

    }
}
