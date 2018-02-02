using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface ISubscriberPreviewDataManager : IDataManager
    {
        long ProcessInstanceId { set; }

        IEnumerable<SubscriberPreview> GetSubscriberPreviews(long processInstanceId);

        bool InsertSubscriberPreview(SubscriberPreview subscriberPreview);
    }
}
