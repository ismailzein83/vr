using System;
using System.Activities;
using System.Data.SqlClient;
using Vanrise.BusinessProcess.Extensions.WFTaskAction.Arguments;
using Vanrise.Common;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess.WFActivities
{
    public class DatabaseJobTaskActivity : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<string> ConnectionString { get; set; }

        [RequiredArgument]
        public InArgument<string> ConnectionStringName { get; set; }

        [RequiredArgument]
        public InArgument<string> Query { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Executing query has started");
            var taskId = context.ActivityContext.GetSharedInstanceData().InstanceInfo.TaskId;
            if (!taskId.HasValue)
                throw new NullReferenceException("Task Id");

            var databaseJobProcessInput = GetDatabaseJobProcessInput(taskId.Value);

            var connectionStringName = databaseJobProcessInput.ConnectionStringName;

            string connectionString;
            if (!String.IsNullOrWhiteSpace(connectionStringName))
                connectionString = Vanrise.Common.Utilities.GetExposedConnectionString(connectionStringName);
            else
                connectionString = databaseJobProcessInput.ConnectionString;

            if (String.IsNullOrWhiteSpace(connectionString))
                throw new NullReferenceException("connectionString");

            var query = databaseJobProcessInput.Query;

            if (!String.IsNullOrWhiteSpace(query))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Executing query is done");
        }

        private DatabaseJobProcessInput GetDatabaseJobProcessInput(Guid taskId)
        {
            SchedulerTask task = new SchedulerTaskManager().GetTask(taskId);
            WFTaskActionArgument wfTaskActionArgument = task.TaskSettings.TaskActionArgument.CastWithValidate<WFTaskActionArgument>("wfTaskActionArgument", taskId);
            DatabaseJobProcessInput databaseJobProcessInput = wfTaskActionArgument.ProcessInputArguments.CastWithValidate<DatabaseJobProcessInput>("databaseJobProcessInput", taskId);

            return databaseJobProcessInput;
        }
    }
}