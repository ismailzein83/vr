using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public abstract class SupplierPriceListSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract ConvertedPriceList Execute(ISupplierPriceListExecutionContext context);
    }
}