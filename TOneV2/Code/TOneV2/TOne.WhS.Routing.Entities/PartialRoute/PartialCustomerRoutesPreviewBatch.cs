using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class PartialCustomerRoutesPreviewBatch
    {
        public PartialCustomerRoutesPreviewBatch()
        {
            this.AffectedPartialCustomerRoutesPreview = new List<ModifiedCustomerRoutePreviewData>();
        }

        public List<ModifiedCustomerRoutePreviewData> AffectedPartialCustomerRoutesPreview { get; set; }
    }
}