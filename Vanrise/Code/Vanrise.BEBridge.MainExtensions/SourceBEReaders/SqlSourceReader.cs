using System.Data;
using System.Data.SqlClient;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class SqlSourceReader : SourceBEReader
    {
        public SqlSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            SqlSourceReaderState state = context.ReaderState as SqlSourceReaderState;

            if (state == null)
                state = new SqlSourceReaderState();

            SqlSourceBatch sourceBatch = new SqlSourceBatch();
            using (SqlConnection connection = new SqlConnection(Setting.ConnectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = Setting.Query;
                command.CommandTimeout = Setting.CommandTimeout;

                DataTable table = new DataTable();

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                sourceBatch.Data = table;
            }
            context.OnSourceBEBatchRetrieved(sourceBatch, null);
        }
    }

    public class SqlSourceReaderSetting
    {
        public string ConnectionString { get; set; }
        public string Query { get; set; }
        public int CommandTimeout { get; set; }
    }

    public class SqlSourceReaderState
    {

    }
}
