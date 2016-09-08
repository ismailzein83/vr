using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Data
{
    public interface IDefaultServicePreviewDataManager : IDataManager
    {
        long ProcessInstanceId { set; }

        bool Insert(DefaultServicePreview preview);

        DefaultServicePreview Get(RatePlanPreviewQuery query);
    }
}
