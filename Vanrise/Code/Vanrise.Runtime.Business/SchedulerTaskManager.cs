using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskManager
    {
        public List<Vanrise.Runtime.Entities.SchedulerTask> GetFilteredTasks(int fromRow, int toRow, string name)
        {
            ISchedulerTaskDataManager datamanager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            return datamanager.GetFilteredTasks(fromRow, toRow, name);
        }
    }
}
