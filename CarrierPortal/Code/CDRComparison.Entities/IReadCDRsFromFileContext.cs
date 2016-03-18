using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public interface IReadCDRsFromFileContext
    {
        bool TryReadLine(out string line);

        byte[] FileContent { get; }

        void OnCDRsReceived(IEnumerable<CDR> cdrs);
    }
}
