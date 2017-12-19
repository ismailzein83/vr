using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public class Pop3SupplierPricelistMessageFilter : VRPop3MessageFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("5D23C45E-49D0-46CD-AED1-173E340070A3"); }
        }

        public override bool IsApplicable()
        {
            throw new NotImplementedException();
        }

    
    }
}
