using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing.Data
{
    public interface IQueueItemHeaderDataManager  : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.QueueItemHeader> GetFilteredQueueItemHeader(Vanrise.Entities.DataRetrievalInput<Entities.QueueItemHeaderQuery> input);
    }
}