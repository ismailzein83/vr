using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Integration.Entities;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Integration.Business
{
    public class DSSchedulerTaskAction : SchedulerTaskAction
    {
        DataSourceManager _dataSourceManager = new DataSourceManager();
        DataSourceRuntimeInstanceManager _dsRuntimeInstanceManager = new DataSourceRuntimeInstanceManager();

        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {           
            Vanrise.Integration.Entities.DataSource dataSource = _dataSourceManager.GetDataSourcebyTaskId(task.TaskId);
            if (dataSource == null)
                throw new NullReferenceException(String.Format("dataSource, Task Id '{0}'", task.TaskId));
            
            IDataSourceLogger logger = new DataSourceLogger();
            logger.DataSourceId = dataSource.DataSourceId;
            logger.WriteInformation("Started running Data Source '{0}'", dataSource.Name);

            _dsRuntimeInstanceManager.AddNewInstance(dataSource.DataSourceId);
            logger.WriteInformation("Runtime Instance created for Data Source: '{0}'", dataSource.Name);

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.WaitingEvent                 
            };
            
            return output;
        }

        public override SchedulerTaskCheckProgressOutput CheckProgress(ISchedulerTaskCheckProgressContext context, int ownerId)
        {
            Vanrise.Integration.Entities.DataSource dataSource = _dataSourceManager.GetDataSourcebyTaskId(context.Task.TaskId);
            if (dataSource == null)
                throw new NullReferenceException(String.Format("dataSource, Task Id '{0}'", context.Task.TaskId));

            if (!_dsRuntimeInstanceManager.DoesAnyDSRuntimeInstanceExist(dataSource.DataSourceId))
            {
                IDataSourceLogger logger = new DataSourceLogger();
                logger.DataSourceId = dataSource.DataSourceId;
                logger.WriteInformation("Finished running Data Source '{0}' ", dataSource.Name);
                return new SchedulerTaskCheckProgressOutput { Result = ExecuteOutputResult.Completed };
            }
            else
            {
                return new SchedulerTaskCheckProgressOutput { Result = ExecuteOutputResult.WaitingEvent };
            }
        }
    }
}
