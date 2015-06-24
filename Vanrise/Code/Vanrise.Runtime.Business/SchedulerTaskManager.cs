using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskManager
    {
        public List<Vanrise.Runtime.Entities.SchedulerTask> GetFilteredTasks(int fromRow, int toRow, string name)
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            return datamanager.GetFilteredTasks(fromRow, toRow, name);
        }

        public Vanrise.Runtime.Entities.SchedulerTask GetTask(int taskId)
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            return datamanager.GetTask(taskId);
        }

        public Vanrise.Entities.InsertOperationOutput<SchedulerTask> AddTask(SchedulerTask taskObject)
        {
            InsertOperationOutput<SchedulerTask> insertOperationOutput = new InsertOperationOutput<SchedulerTask>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int taskId = -1;

            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            bool insertActionSucc = dataManager.AddTask(taskObject, out taskId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                taskObject.TaskId = taskId;
                insertOperationOutput.InsertedObject = taskObject;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<SchedulerTask> UpdateTask(SchedulerTask taskObject)
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();

            bool updateActionSucc = dataManager.UpdateTask(taskObject);
            UpdateOperationOutput<SchedulerTask> updateOperationOutput = new UpdateOperationOutput<SchedulerTask>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = taskObject;
            }
            return updateOperationOutput;
        }

    }
}
