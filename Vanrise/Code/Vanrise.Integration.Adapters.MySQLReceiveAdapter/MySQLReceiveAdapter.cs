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
using Vanrise.Common;

namespace Vanrise.Integration.Adapters.MySQLReceiveAdapter
{
    public class MySQLReceiveAdapter : BaseDBReceiveAdapter
    {
        protected override DbConnection CreateDBConnection(string connString)
        {
            return new MySqlConnection(connString);
        }

        protected override DbCommand CreateDBCommand(string queryText, DbConnection connection)
        {
            return new MySqlCommand(queryText, connection.CastWithValidate<MySqlConnection>("connection"));
        }

        protected override void DefineRangeParameters(DbCommand command, DBReceiveAdapterRangeDBType rangeDBType)
        {
            MySqlDbType mySQLDbType = rangeDBType == DBReceiveAdapterRangeDBType.BigInt ? MySqlDbType.Int64 : MySqlDbType.DateTime;
            MySqlCommand mySQLCommand = command.CastWithValidate<MySqlCommand>("command");
            mySQLCommand.Parameters.Add("@RangeStart", mySQLDbType);
            mySQLCommand.Parameters.Add("@RangeEnd", mySQLDbType);
        }

        protected override void SetRangeParameterValues(DbCommand command, object rangeStart, object rangeEnd)
        {
            MySqlCommand mySQLCommand = command.CastWithValidate<MySqlCommand>("command");
            mySQLCommand.Parameters["@RangeStart"].Value = rangeStart;
            mySQLCommand.Parameters["@RangeEnd"].Value = rangeEnd;
        }

        protected override string GetDBQueryTopOneStatement()
        {
            return "Limit 1";
        }
    }
}
