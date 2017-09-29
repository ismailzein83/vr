using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailAttachmentGeneral : VRMailAttachement
    {
        public override System.Net.Mime.ContentType ContentType
        {
            get { return new ContentType(); }
        }
        public override System.Net.Mime.TransferEncoding TransferEncoding
        {
            get { return TransferEncoding.Base64; }
        }

        public override Encoding NameEncoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
