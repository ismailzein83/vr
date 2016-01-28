using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueItemTypeManager
    {
        public List<QueueItemType> GetItemTypes()
        {
            IQueueItemTypeDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemTypeDataManager>();
            return manager.GetItemTypes();
            
        }
        
    }

     
}