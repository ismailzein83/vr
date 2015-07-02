using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueingManager
    {
        private IQueueDataManager _dataManager;

        public QueueingManager()
        {
            _dataManager = QDataManagerFactory.GetDataManager<IQueueDataManager>();
        }

        public List<QueueItemType> GetQueueItemTypes()
        {
            return _dataManager.GetQueueItemTypes();
        }

        public List<QueueInstance> GetQueueInstances(IEnumerable<int> queueItemTypes)
        {
            return _dataManager.GetQueueInstancesByTypes(queueItemTypes);
        }

    }
}
