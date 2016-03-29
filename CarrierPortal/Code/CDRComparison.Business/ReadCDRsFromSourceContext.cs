using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Business
{
    public class ReadCDRsFromSourceContext : IReadCDRsFromSourceContext
    {
        Action<IEnumerable<CDR>> _onCDRsReceived;

        public ReadCDRsFromSourceContext(Action<IEnumerable<CDR>> onCDRsReceived)
        {
            this._onCDRsReceived = onCDRsReceived;
        }

        public void OnCDRsReceived(IEnumerable<CDR> cdrs)
        {
            this._onCDRsReceived(cdrs);
        }
    }
}
