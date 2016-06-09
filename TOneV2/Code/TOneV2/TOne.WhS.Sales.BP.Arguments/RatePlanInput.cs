using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Arguments
{
    public class RatePlanInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int CurrencyId { get; set; }

        public override string GetTitle()
        {
            string ownerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(this.OwnerType);
            return String.Format("Started the Rate Plan process for {0} '{1}'", ownerTypeDescription, this.OwnerId);
        }
    }
}
