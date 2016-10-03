using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.SummaryTransformation
{
    public interface ISummaryItem
    {
        long SummaryItemId { get; set; }

        DateTime BatchStart { set; }

        DateTime BatchEnd { set; }
    }
}
