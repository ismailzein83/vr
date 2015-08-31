using System;
using System.Data.SqlClient;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Adapters.SQLReceiveAdapter
{

    public class SQLReceiveAdapter : BaseReceiveAdapter
    {
        public override void ImportData(int dataSourceId, BaseAdapterState adapterState, BaseAdapterArgument argument, Action<IImportedData> receiveData)
        {
            DBAdapterArgument dbAdapterArgument = argument as DBAdapterArgument;
            DBAdapterState dbAdapterState = adapterState as DBAdapterState;
            
            DBReaderImportedData data = null;

            try
            {
                using (var connection = new SqlConnection(dbAdapterArgument.ConnectionString))
                {
                    connection.Open();
                    bool keepFetchingData = false;
                    do
                    {
                        string query = dbAdapterArgument.Query.ToLower().Replace("{lastimportedid}", dbAdapterState.LastImportedId);
                        var command = new SqlCommand(query, connection);

                        data = new DBReaderImportedData();
                        data.Reader = command.ExecuteReader();

                        if (keepFetchingData = data.Reader.HasRows)
                        {
                            data.LastImportedId = dbAdapterState.LastImportedId;

                            receiveData(data);

                            if (data.LastImportedId == dbAdapterState.LastImportedId)
                                break;

                            dbAdapterState.LastImportedId = data.LastImportedId;
                            base.UpdateAdapterState(dataSourceId, dbAdapterState);
                        }
                        else
                        {
                            LogInformation("No more rows to fetch from Source DB");
                        }

                    } while (keepFetchingData);
                    
                }
            }
            catch (Exception ex)
            {
                LogError("An error occurred in SQL Adapter while importing data. Exception Details: {0}", ex.Message);
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
