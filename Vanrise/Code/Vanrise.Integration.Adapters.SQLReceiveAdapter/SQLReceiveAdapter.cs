using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Vanrise.Integration.Adapters.DBReceiveAdapter.Arguments;
using Vanrise.Integration.Entities;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;
using System.Data.Common;
using Vanrise.Common;

namespace Vanrise.Integration.Adapters.SQLReceiveAdapter
{
    public class SQLReceiveAdapter : BaseDBReceiveAdapter
    {
        protected override DbConnection CreateDBConnection(string connString)
        {
            return new SqlConnection(connString);
        }

        protected override DbCommand CreateDBCommand(string queryText, DbConnection connection)
        {
            return new SqlCommand(queryText, connection.CastWithValidate<SqlConnection>("connection"));
        }

        protected override void DefineRangeParameters(DbCommand command, DBReceiveAdapterRangeDBType rangeDBType)
        {
            System.Data.SqlDbType sqlDbType = rangeDBType == DBReceiveAdapterRangeDBType.BigInt ? System.Data.SqlDbType.BigInt : System.Data.SqlDbType.DateTime;
            SqlCommand sqlCommand = command.CastWithValidate<SqlCommand>("command");
            sqlCommand.Parameters.Add("@RangeStart", sqlDbType);
            sqlCommand.Parameters.Add("@RangeEnd", sqlDbType);
        }

        protected override void SetRangeParameterValues(DbCommand command, object rangeStart, object rangeEnd)
        {
            SqlCommand sqlCommand = command.CastWithValidate<SqlCommand>("command");
            sqlCommand.Parameters["@RangeStart"].Value = rangeStart;
            sqlCommand.Parameters["@RangeEnd"].Value = rangeEnd;
        }

        protected override string GetDBQueryTopOneStatement()
        {
            return "TOP 1";
        }
    }
}