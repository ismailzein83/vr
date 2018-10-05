using System;
using System.Activities;
using System.Data.SqlClient;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public class DatabaseJobTaskActivity : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<string> ConnectionString { get; set; }

        [RequiredArgument]
        public InArgument<string> ConnectionStringName { get; set; }

        [RequiredArgument]
        public InArgument<string> CustomCode { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            var connectionStringName = this.ConnectionStringName.Get(context.ActivityContext);

            string connectionString;
            if (!String.IsNullOrWhiteSpace(connectionStringName))
                connectionString = Vanrise.Common.Utilities.GetExposedConnectionString(connectionStringName);
            else
                connectionString = this.ConnectionString.Get(context.ActivityContext);

            if (String.IsNullOrWhiteSpace(connectionString))
                throw new NullReferenceException("connectionString");

            var customCode = this.CustomCode.Get(context.ActivityContext);

            if (!String.IsNullOrWhiteSpace(customCode))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(customCode, connection))
                    {
                        int executed = command.ExecuteNonQuery();
                        if (executed < 0)
                            context.ActivityContext.WriteTrackingMessage(LogEntryType.Error, "Query failed to be executed");
                    }

                    connection.Close();
                }
            }
        }
    }
}