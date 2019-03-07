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
        public InArgument<string> ConnectionStringAppSettingName { get; set; }

        [RequiredArgument]
        public InArgument<string> Query { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            context.ActivityContext.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Executing query has started");
            var taskId = context.ActivityContext.GetSharedInstanceData().InstanceInfo.TaskId;
            if (!taskId.HasValue)
                throw new NullReferenceException("Task Id");

            var databaseJobProcessInput = GetDatabaseJobProcessInput(taskId.Value);

            string connectionString;
            if (!String.IsNullOrWhiteSpace(databaseJobProcessInput.ConnectionString))
            {
                connectionString = databaseJobProcessInput.ConnectionString;
            }
            else if (!String.IsNullOrWhiteSpace(databaseJobProcessInput.ConnectionStringName))
            {
                connectionString = Utilities.GetConnectionStringByName(databaseJobProcessInput.ConnectionStringName);
            }
            else
            {
                connectionString = Utilities.GetConnectionStringByAppSettingName(databaseJobProcessInput.ConnectionStringAppSettingName);
            }

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
                        command.CommandTimeout = 0;
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
            return wfTaskActionArgument.ProcessInputArguments.CastWithValidate<DatabaseJobProcessInput>("databaseJobProcessInput", taskId);
        }
    }
}