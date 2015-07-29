using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public enum CarrierType : short
    {
        Exchange = 0,
        Customer = 1,
        Supplier = 2
    }

    public enum Change : short
    {
        None = 0,
        Increase = 1,
        Decrease = -1,
        New = 2
    }

    public enum ToDRateType : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2,
        Holiday = 4
    }

    public enum CarrierTypeFilter : short
    {
        All = 0,
        OnlyCustomers = 1,
        OnlySuppliers = 2
    }
}
