using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public abstract class InputPriceListSettings
    {
        public int ConfigId { get; set; }

        public abstract PriceList Execute(IInputPriceListExecutionContext context);
    }
}
