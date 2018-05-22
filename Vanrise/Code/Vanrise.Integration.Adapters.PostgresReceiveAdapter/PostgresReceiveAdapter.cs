using System;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
using Vanrise.Integration.Adapters.DBBaseReceiveAdapter;
using Vanrise.Integration.Adapters.PostgresReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using Vanrise.Common;

namespace Vanrise.Integration.Adapters.PostgresReceiveAdapter
{
    public class PostgresReceiveAdapter : BaseDBReceiveAdapter
    {
        protected override DbConnection CreateDBConnection(string connString)
        {
            return new NpgsqlConnection(connString);
        }

        protected override DbCommand CreateDBCommand(string queryText, DbConnection connection)
        {
            return new Npgsql.NpgsqlCommand(queryText, connection.CastWithValidate<NpgsqlConnection>("connection"));
        }

        protected override void DefineRangeParameters(DbCommand command, DBReceiveAdapterRangeDBType rangeDBType)
        {
            NpgsqlDbType postgresDbType = rangeDBType == DBReceiveAdapterRangeDBType.BigInt ? NpgsqlDbType.Bigint : NpgsqlDbType.Date;
            NpgsqlCommand postgresCommand = command.CastWithValidate<NpgsqlCommand>("command");
            postgresCommand.Parameters.Add("@RangeStart", postgresDbType);
            postgresCommand.Parameters.Add("@RangeEnd", postgresDbType);
        }

        protected override void SetRangeParameterValues(DbCommand command, object rangeStart, object rangeEnd)
        {
            NpgsqlCommand postgresCommand = command.CastWithValidate<NpgsqlCommand>("command");
            postgresCommand.Parameters["@RangeStart"].Value = rangeStart;
            postgresCommand.Parameters["@RangeEnd"].Value = rangeEnd;
        }

        protected override string GetDBQueryTopOneStatement()
        {
            return "Limit 1";
        }
    }
}
