using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class ExistingDefaultService : Vanrise.Entities.IDateEffectiveSettings
    {
        public SaleEntityDefaultService SaleEntityDefaultServiceEntity { get; set; }
        public ChangedDefaultService ChangedDefaultService { get; set; }
        public DateTime BED
        {
            get { return SaleEntityDefaultServiceEntity.BED; }
        }
        public DateTime? EED
        {
            get { return ChangedDefaultService != null ? ChangedDefaultService.EED : SaleEntityDefaultServiceEntity.EED; }
        }
    }
}
