using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Arguments
{
    public class RatePlanInputManager
    {
        public string GetTitle(SalePriceListOwnerType ownerType, int ownerId)
        {
            string ownerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(ownerType);
            string ownerName;

            switch (ownerType)
            {
                case SalePriceListOwnerType.SellingProduct:
                    var sellingProductManager = new SellingProductManager();
                    ownerName = sellingProductManager.GetSellingProductName(ownerId);
                    break;
                case SalePriceListOwnerType.Customer:
                    var carrierAccountManager = new CarrierAccountManager();
                    ownerName = carrierAccountManager.GetCarrierAccountName(ownerId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return String.Format("Started the #BPDefinitionTitle# process for {0} '{1}'", ownerTypeDescription, ownerName);
        }
    }
}
