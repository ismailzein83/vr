using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data;
using TOne.WhS.RouteSync.Radius;

namespace TOne.WhS.RouteSync.TelesRadius.Postgres
{
    public class RadiusPostgresDataManager : BaseDataManager, IRadiusDataManager
    {
        public Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public void PrepareTables()
        {
            throw new NotImplementedException();
        }

        public void InsertRoutes(List<ConvertedRoute> radiusRoutes)
        {
            throw new NotImplementedException();
        }

        public void SwapTables()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(""))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 10 * 60;
                    DateTime date = DateTime.Now;
                    command.CommandText = "";
                    command.ExecuteNonQuery();
                }
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = 30;
                    DateTime date = DateTime.Now;
                    command.CommandText = "";
                    command.ExecuteNonQuery();
                    TimeSpan spent = DateTime.Now.Subtract(date);
                }
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }
    }
}
