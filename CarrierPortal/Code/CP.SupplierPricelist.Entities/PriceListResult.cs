using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public enum PriceListResult
    {
        [Description(" ")]
        NotCompleted = 0,

        Imported = 10,

        Rejected = 20,

        [Description("Partially Rejected")]
        PartiallyRejected = 30
    }
}
