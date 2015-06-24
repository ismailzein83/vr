using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Data
{
    public interface ISchedulerTaskDataManager : IDataManager
    {
        List<Entities.SchedulerTask> GetFilteredTasks(int fromRow, int toRow, string name);

        Entities.SchedulerTask GetTask(int taskId);

        bool AddTask(Entities.SchedulerTask taskObject, out int insertedId);

        bool UpdateTask(Entities.SchedulerTask taskObject);
    }
}
