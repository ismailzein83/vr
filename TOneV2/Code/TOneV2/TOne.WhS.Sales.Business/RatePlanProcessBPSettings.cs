using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.BP.Arguments;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RatePlanInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<RatePlanInput>("context.IntanceToRun.InputArgument");
            CarrierAccountManager manager = new CarrierAccountManager();
            var sellingProductId = manager.GetSellingProductId(inputArg.OwnerId);
            var carrierAccountIds = manager.GetCarrierAccountIdsAssignedToSellingProduct(inputArg.OwnerId);
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RatePlanInput startedBPInstanceInputArg = startedBPInstance.InputArgument as RatePlanInput;
                if (startedBPInstanceInputArg != null)
                {
                    if (inputArg.OwnerType == SalePriceListOwnerType.Customer)
                    {
                        if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.Customer && startedBPInstanceInputArg.OwnerId == inputArg.OwnerId)
                        {
                            context.Reason = "Another process is running for the same Customer";
                            return false;
                        }

                        if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.SellingProduct && sellingProductId == startedBPInstanceInputArg.OwnerId)
                        {
                            context.Reason = "Another process is running for customer's sellingProuct";
                            return false;
                        }
                    }
                    else
                    {
                        if (inputArg.OwnerType == SalePriceListOwnerType.SellingProduct)
                        {
                            if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.SellingProduct && startedBPInstanceInputArg.OwnerId == inputArg.OwnerId)
                            {
                                context.Reason = "Another process is running for the same sellingProduct";
                                return false;
                            }
                            foreach(var carrierAccountId in carrierAccountIds)
                            {
                                if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.Customer && carrierAccountId == startedBPInstanceInputArg.OwnerId)
                                {
                                    context.Reason = "A process is running of type customer for the same sellingProduct";
                                    return false;
                                }
                            }
                        }
                    }
                    
                }
            }
            return true;
        }
    }
}
