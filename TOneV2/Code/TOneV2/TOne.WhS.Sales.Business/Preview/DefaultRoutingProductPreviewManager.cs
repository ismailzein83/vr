using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class DefaultRoutingProductPreviewManager
    {
        public DefaultRoutingProductPreview GetDefaultRoutingProductPreview(RatePlanPreviewQuery query)
        {
            var dataManager = SalesDataManagerFactory.GetDataManager<IDefaultRoutingProductPreviewDataManager>();
            return dataManager.Get(query);
        }
    }
}
