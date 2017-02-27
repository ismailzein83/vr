using System.Data;
using System.Data.SqlClient;
using Vanrise.BEBridge.Entities;
using Vanrise.Common.Business;
using System.Linq;
using System;

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
            SQLConnection settings = new VRConnectionManager().GetVRConnection(Setting.VRConnectionId).Settings as SQLConnection;
            string connectionString = (settings!=null)?settings.ConnectionString : null;
            if (String.IsNullOrEmpty(connectionString))
                throw new NullReferenceException(String.Format("connection string is null or empty"));
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = Setting.Query;
                command.CommandTimeout = Setting.CommandTimeout;
                if (Setting.BasedOnId)
                    command.Parameters.Add(new SqlParameter { ParameterName = "LastImportedId", Value = state.LastImportedId });

                DataTable table = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                sourceBatch.Data = table;
                if (Setting.BasedOnId)
                {
                    var maxIdValue = table.Compute(string.Format("max({0})", Setting.IdField), string.Empty);
                    state.LastImportedId = maxIdValue == DBNull.Value ? state.LastImportedId : Convert.ToInt64(maxIdValue);
                }
            }
            context.ReaderState = state;
            context.OnSourceBEBatchRetrieved(sourceBatch, null);

        }
    }

    public class SqlSourceReaderSetting
    {
        public Guid VRConnectionId { get; set; }
        public string Query { get; set; }
        public int CommandTimeout { get; set; }
        public bool BasedOnId { get; set; }
        public string IdField { get; set; }
    }

    public class SqlSourceReaderState
    {
        public long LastImportedId { get; set; }
    }
}
