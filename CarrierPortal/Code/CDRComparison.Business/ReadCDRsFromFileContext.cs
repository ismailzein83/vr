using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace CDRComparison.Business
{
    public class ReadCDRsFromFileContext : ReadFromFileContext, IReadCDRsFromFileContext
    {       
        Action<IEnumerable<CDR>> _onCDRsReceived;

        public ReadCDRsFromFileContext(VRFile file, Action<IEnumerable<CDR>> onCDRsReceived) : base(file)
        {
            if (onCDRsReceived == null)
                throw new ArgumentNullException("onCDRsReceived");
            this._onCDRsReceived = onCDRsReceived;
        }

        public void OnCDRsReceived(IEnumerable<CDR> cdrs)
        {
            this._onCDRsReceived(cdrs);
        }
    }
}
