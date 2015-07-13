using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public enum TimePeriod
    {
        Days = 0,
        Weeks = 1,
        Months = 2
    }

    public enum VariationReportOptions
    {
        InBoundMinutes = 0,
        OutBoundMinutes = 1,
        InOutBoundMinutes = 2,
        TopDestinationMinutes = 3,
        InBoundAmount = 4,
        OutBoundAmount = 5,
        InOutBoundAmount = 6,
        TopDestinationAmount = 7,
        Profit = 8
    }

    public enum EntityType
    {
        none = 0,
        Customer = 1,
        Supplier = 2,
        Zone = 3,
        Profit =4

    }

    public enum GroupingBy
    {
        none = 0,
        Customers = 1,
        Suppliers = 2

    }

    public enum AccountType : byte
    {
        Client = 0,     // Client only
        Exchange = 1,   // Client and Supply
        Termination = 2 // Supply Only
    }

}

