using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments;
using System.Data.Common;
using Npgsql;
using Vanrise.Integration.Adapters.DBReceiveAdapter;
using NpgsqlTypes;
using Vanrise.Runtime;

namespace Vanrise.Integration.Adapters.PostgresReceiveAdapter
{
    public class PostgresReceiveAdapter : DbBaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            PostgresAdapterArgument postgresAdapterArgument = context.AdapterArgument as PostgresAdapterArgument;
            if (postgresAdapterArgument == null)
                throw new NullReferenceException("postgresAdapterArgument");

            RunInParallelMode(context, postgresAdapterArgument);
        }

        public void RunInParallelMode(IAdapterImportDataContext context, DbBaseAdapterArgument dbAdapterArgument)
        {
            bool isLastRange;
            DbAdapterRangeState rangeToRead = GetAndLockNextRangeToRead(context, null, dbAdapterArgument, out isLastRange);

            if (rangeToRead == null)
            {
                LogInformation("No More Ranges to read");
                return;
            }

            string queryWithTop1 = dbAdapterArgument.Query.Replace("#TopRows#", "Limit 1");
            string queryWithNoTop = dbAdapterArgument.Query.Replace("#TopRows#", "");

            DBReaderImportedData data = null;
            DbConnection connection = null;

            try
            {
                DbCommand command = null;

               
                connection = new NpgsqlConnection(dbAdapterArgument.ConnectionString);
                connection.Open();
                command = new Npgsql.NpgsqlCommand(queryWithNoTop, connection as NpgsqlConnection);
                command.CommandTimeout = dbAdapterArgument.CommandTimeoutInSeconds != default(int) ? dbAdapterArgument.CommandTimeoutInSeconds : 600;
                DefineParameters(command, dbAdapterArgument);

                do
                {
                    try
                    {
                        Console.WriteLine("{0}: Reading Range {1} - {2}", DateTime.Now, rangeToRead.RangeStart, rangeToRead.RangeEnd);
                        ReadRange(context, dbAdapterArgument, isLastRange, rangeToRead, queryWithTop1, queryWithNoTop, ref data, command);
                    }
                    finally
                    {
                        ReleaseRange(context, rangeToRead);
                    }

                    rangeToRead = GetAndLockNextRangeToRead(context, rangeToRead, dbAdapterArgument, out isLastRange);
                    if (rangeToRead == null)
                        LogInformation("No More Ranges to read");
                }
                while (rangeToRead != null);
            }
            catch (Exception ex)
            {
                LogError("An error occurred in SQL Adapter while importing data. Exception Details: {0}", ex.ToString());
            }
            finally
            {
                if (data != null)
                    data.OnDisposed();
                if (connection != null)
                {
                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                    connection.Dispose();
                }
            }
        }

        public override void DefineParameters(DbCommand command, DbBaseAdapterArgument dbAdapterArgument)
        {
            NpgsqlCommand npgCommand = command as NpgsqlCommand;
            npgCommand.Parameters.Add("@RangeStart", GetRangePrmPostgresType(dbAdapterArgument));
            npgCommand.Parameters.Add("@RangeEnd", GetRangePrmPostgresType(dbAdapterArgument));
        }

        #region Private Methods

        private NpgsqlDbType GetRangePrmPostgresType(DbBaseAdapterArgument dbAdapterArgument)
        {
            if (dbAdapterArgument.NumberOffSet.HasValue)
                return NpgsqlDbType.Bigint;
            else if (dbAdapterArgument.TimeOffset.HasValue)
                return NpgsqlDbType.Date;
            else
                throw new Exception("PostgresAdapterArgument doesnt have any Offset defined");
        }

        #endregion
    }
}
