using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;
using System.Data.Common;
using Vanrise.Runtime;
using Vanrise.Integration.Adapters.DBBaseReceiveAdapter;
using Vanrise.Integration.Adapters.MySQLReceiveAdapter.Arguments;
using MySql.Data.MySqlClient;

namespace Vanrise.Integration.Adapters.MySQLReceiveAdapter
{
    public class MySQLReceiveAdapter : DbBaseReceiveAdapter
    {
        public override void ImportData(IAdapterImportDataContext context)
        {
            MySQLAdapterArgument mySQLAdapterArgument = context.AdapterArgument as MySQLAdapterArgument;
            if (mySQLAdapterArgument == null)
                throw new NullReferenceException("mySQLAdapterArgument");

            RunInParallelMode(context, mySQLAdapterArgument);
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


                connection = new MySqlConnection(dbAdapterArgument.ConnectionString);
                connection.Open();
                command = new MySqlCommand(queryWithNoTop, connection as MySqlConnection);
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
            MySqlCommand npgCommand = command as MySqlCommand;
            npgCommand.Parameters.Add("@RangeStart", GetRangePrmPostgresType(dbAdapterArgument));
            npgCommand.Parameters.Add("@RangeEnd", GetRangePrmPostgresType(dbAdapterArgument));
        }

        #region Private Methods

        private MySqlDbType GetRangePrmPostgresType(DbBaseAdapterArgument dbAdapterArgument)
        {
            if (dbAdapterArgument.NumberOffSet.HasValue)
                return MySqlDbType.Int64;
            else if (dbAdapterArgument.TimeOffset.HasValue)
                return MySqlDbType.Date;
            else
                throw new Exception("MySQLAdapterArgument doesnt have any Offset defined");
        }

        #endregion
    }
}
