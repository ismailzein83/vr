using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueItemManager
    {

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            IQueueItemDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            return manager.GetItemStatusSummary();
        
        }

    }
}
