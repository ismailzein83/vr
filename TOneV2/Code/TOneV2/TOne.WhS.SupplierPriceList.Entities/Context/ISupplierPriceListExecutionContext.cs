using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public interface ISupplierPriceListExecutionContext
    {
        long InputFileId { get; }
        DateTime PricelistDate { get; }
    }
}
