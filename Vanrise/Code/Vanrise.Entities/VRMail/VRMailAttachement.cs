using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRMailAttachement
    {
        public string Name { get; set; }

        public byte[] Content { get; set; }

        public abstract ContentType ContentType { get; }

        public abstract TransferEncoding TransferEncoding { get; }

        public abstract Encoding NameEncoding { get; }
    }
}
