using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BEBridge.Entities
{
    public class FileSourceBatch : SourceBEBatch
    {
        public override string BatchName
        {
            get { return this.FileName; }
        }

        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }
}
