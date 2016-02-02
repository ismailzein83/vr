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

        PartiallyApproved = 20,

        Rejected = 30

    }
}
