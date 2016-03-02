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
        [Description("Not Completed")]
        NotCompleted = 0,

        Approved = 10,

        Rejected = 20,

        [Description("Partially Rejected")]
        PartiallyRejected = 30,
        Completed = 40

    }
}
