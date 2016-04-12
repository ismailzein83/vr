using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public interface IReadCDRsFromSourceContext
    {
        void OnCDRsReceived(IEnumerable<CDR> cdrs);
    }
}
