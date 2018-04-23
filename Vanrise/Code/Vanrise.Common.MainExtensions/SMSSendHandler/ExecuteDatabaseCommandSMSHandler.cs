using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions.SMSSendHandler
{
    public class ExecuteDatabaseCommandSMSHandler : SMSSendHandlerSettings
    {
        public override Guid ConfigId
        {
            get
            {
                return new Guid("44E97625-1B35-478A-918E-60F9C58678B4");
            }
        }
        public Guid VRConnectionId { get; set; }
        public string CommandQuery { get; set; }

        public override void SendSMS(ISMSHandlerSendSMSContext context)
        {

            SQLConnection settings = new VRConnectionManager().GetVRConnection(VRConnectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;
            if (String.IsNullOrEmpty(connectionString))
                throw new NullReferenceException(String.Format("connection string is null or empty"));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();

            //CommandQuery replace message and number in query 
                string newCommandQuery = CommandQuery;
                newCommandQuery = newCommandQuery.Replace("#MobileNumber#", context.MobileNumber);
                newCommandQuery = newCommandQuery.Replace("#Message#", context.Message);
                newCommandQuery = newCommandQuery.Replace("#MessageTime#", context.MessageTime.ToString());

                var command = connection.CreateCommand();
                command.CommandText = newCommandQuery;
                command.CommandTimeout = 60;
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
                connection.Close();
            }
          
        }
      
    }
}
