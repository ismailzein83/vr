using Retail.Dealers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.MainExtensions.DealerTypes.Reseller
{
    public class ResellerTypeSettings : DealerTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1B81FC90-C9E1-464B-9D03-3F6D4A0FBEC0"); }
        }

        public override string RuntimeEditor
        {
            get { throw new NotImplementedException(); }
        }
    }
}
