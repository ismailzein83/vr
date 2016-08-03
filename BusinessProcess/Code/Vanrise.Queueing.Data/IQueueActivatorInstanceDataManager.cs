using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueActivatorInstanceDataManager : IDataManager
    {
        void InsertActivator(Guid activatorId, int runtimeProcessId, QueueActivatorType activatorType, string serviceURL);

        List<QueueActivatorInstance> GetAll();

        void Delete(Guid activatorId);
    }
}
