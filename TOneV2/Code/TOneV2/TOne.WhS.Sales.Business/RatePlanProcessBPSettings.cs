using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.BP.Arguments;
using TOne.WhS.Sales.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            IEnumerable<BPInstance> startedBPInstances = context.GetStartedBPInstances();
            if (startedBPInstances == null || startedBPInstances.Count() == 0)
                return true;

            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RatePlanInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<RatePlanInput>("context.IntanceToRun.InputArgument");

            string reason;
            bool canRunBPInstance;

            var carrierAccountManager = new CarrierAccountManager();

            if (inputArg.OwnerType == SalePriceListOwnerType.SellingProduct)
                canRunBPInstance = CanRunBPInstanceForSellingProduct(inputArg.OwnerId, startedBPInstances, carrierAccountManager, out reason);
            else
                canRunBPInstance = CanRunBPInstanceForCustomer(inputArg.OwnerId, inputArg.SubscriberOwnerIds, startedBPInstances, carrierAccountManager, out reason);

            context.Reason = reason;
            return canRunBPInstance;
        }

        public override void OnBPExecutionCompleted(Vanrise.BusinessProcess.Entities.IBPDefinitionBPExecutionCompletedContext context)
        {
            if (context.BPInstance.Status != BPInstanceStatus.Completed)
            {
                long processInstanceId = context.BPInstance.ProcessInstanceID;
                RatePlanManager ratePlanManager = new RatePlanManager();
                ratePlanManager.CleanTemporaryTables(processInstanceId);
            }
            base.OnBPExecutionCompleted(context);
        }

        #region Private Methods

        private bool CanRunBPInstanceForSellingProduct(int sellingProductId, IEnumerable<BPInstance> startedBPInstances, CarrierAccountManager carrierAccountManager, out string reason)
        {
            reason = null;
            IEnumerable<int> carrierAccountIds = carrierAccountManager.GetCarrierAccountIdsAssignedToSellingProduct(sellingProductId);

            foreach (BPInstance startedBPInstance in startedBPInstances)
            {
                RatePlanInput startedBPInstanceInputArg = startedBPInstance.InputArgument as RatePlanInput;
                if (startedBPInstanceInputArg == null)
                    continue;

                if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.SellingProduct && startedBPInstanceInputArg.OwnerId == sellingProductId)
                {
                    reason = "Another process is running for the same selling product";
                    return false;
                }

                foreach (int carrierAccountId in carrierAccountIds)
                {
                    if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.Customer && carrierAccountId == startedBPInstanceInputArg.OwnerId)
                    {
                        reason = "A process is running of type customer for the same selling product";
                        return false;
                    }
                }
            }

            return true;
        }
        private bool CanRunBPInstanceForCustomer(int customerId, IEnumerable<int> subscriberOwnerIds, IEnumerable<BPInstance> startedBPInstances, CarrierAccountManager carrierAccountManager, out string reason)
        {
            reason = null;
            int sellingProductId = carrierAccountManager.GetSellingProductId(customerId);

            foreach (BPInstance startedBPInstance in startedBPInstances)
            {
                RatePlanInput startedBPInstanceInputArg = startedBPInstance.InputArgument as RatePlanInput;
                if (startedBPInstanceInputArg == null)
                    continue;

                if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.Customer)
                {
                    if (startedBPInstanceInputArg.OwnerId == customerId)
                    {
                        reason = "Another process is running for the same customer";
                        return false;
                    }

                    if (subscriberOwnerIds.Contains(startedBPInstanceInputArg.OwnerId))
                    {
                        var customerName = carrierAccountManager.GetCarrierAccountName(startedBPInstanceInputArg.OwnerId);
                        reason = string.Format("Another process is running for customer {0}", customerName);
                        return false;
                    }
                }

                if (startedBPInstanceInputArg.OwnerType == SalePriceListOwnerType.SellingProduct && sellingProductId == startedBPInstanceInputArg.OwnerId)
                {
                    reason = "Another process is running for customer's selling product";
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
