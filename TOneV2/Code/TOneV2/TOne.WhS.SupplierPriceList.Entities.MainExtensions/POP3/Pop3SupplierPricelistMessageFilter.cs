using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions
{
    public class Pop3SupplierPricelistMessageFilter : VRPop3MessageFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("05382832-5CBB-46C0-8214-B3B81769FB80"); }
        }
        
        public override bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader)
        {
            return true;
        }
    }
}
