using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

using TOne.WhS.BusinessEntity.Business;

using TOne.WhS.Sales.Entities;


namespace TOne.WhS.Sales.BP.Arguments
{
    public class RatePlanSubProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public bool IsAdditionaOwner { get; set; }

        public int CurrencyId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public Changes Changes { get; set; }

        public override string EntityId
        {
            get
            {
                return String.Format("{0}_{1}", (int)OwnerType, OwnerId);
            }
        }

        public override string GetTitle()
        {
            string ownerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription(this.OwnerType);
            string ownerName;

            switch (OwnerType)
            {
                case SalePriceListOwnerType.SellingProduct:
                    var sellingProductManager = new SellingProductManager();
                    ownerName = sellingProductManager.GetSellingProductName(OwnerId);
                    break;
                case SalePriceListOwnerType.Customer:
                    var carrierAccountManager = new CarrierAccountManager();
                    ownerName = carrierAccountManager.GetCarrierAccountName(OwnerId);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return String.Format("Started the #BPDefinitionTitle# process for {0} '{1}'", ownerTypeDescription, ownerName);
        }
    }
}
