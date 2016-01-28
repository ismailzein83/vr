using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
   public interface IQueueItemTypeDataManager:IDataManager
    {
       List<QueueItemType> GetItemTypes();

    }
}
