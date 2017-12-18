using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions.POP3
{
    public class Pop3SupplierPricelistFilter : VRPop3Filter
    {
        public Guid ConfigId { get { return new Guid("5D23C45E-49D0-46CD-AED1-173E340070A3"); } }

        public bool IsApplicable ()
        {
            return true;
        }
    }
}
