using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.BEBridge.MainExtensions.SourceBEReaders
{
    public class Pop3SourceReader : SourceBEReader
    {
        public Pop3SourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {

        }
    }

    public class Pop3SourceReaderSetting
    {
        public Guid VRConnectionId { get; set; }

        public Guid Pop3FilterId { get; set; }
    }
}
