using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public enum ComparisonResult
    {
        [Description("Missing System")]
        MissingSystem = 1,

        [Description("Missing")]
        MissingProvider = 2,

        [Description("Identical")]
        Identical = 3,

        [Description("Minor Difference")]
        MinorDiff = 4,

        [Description("Major Difference")]
        MajorDiff = 5
    }
}
