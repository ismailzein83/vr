using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public enum AlertLevel : byte
    {
        Low,
        Medium,
        High,
        Urgent,
        Critical
    }

    public enum AlertProgress : short
    {
        None = 0,
        Positive = 1,
        Negative = -1
    }

    public enum CarrierType : short
    {
        Exchange = 0,
        Customer = 1,
        Supplier = 2
    }
}
