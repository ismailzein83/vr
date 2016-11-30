using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRMailAttachmentPDF : VRMailAttachement
    {
        public override System.Net.Mime.ContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Net.Mime.TransferEncoding TransferEncoding
        {
            get { throw new NotImplementedException(); }
        }

        public override Encoding NameEncoding
        {
            get { throw new NotImplementedException(); }
        }
    }
}
