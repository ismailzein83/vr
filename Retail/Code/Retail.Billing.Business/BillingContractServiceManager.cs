using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class BillingContractServiceManager
    {
        static readonly Guid BeDefinitionId = new Guid("16289a1d-7e2a-4e95-b0c1-fe5775a8efed");

        public IEnumerable<BillingContractService> GetBillingContractServices(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;
            List<BillingContractService> billingContractServices = new List<BillingContractService>();

            return entities.MapRecords(BillingContractServiceMapper);
        }

        private BillingContractService BillingContractServiceMapper(GenericBusinessEntity genericBusinessEntity)
        {
            BillingContractService billingContractService = new BillingContractService();
            billingContractService.ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID");
            billingContractService.ContractID = (long)genericBusinessEntity.FieldValues.GetRecord("Contract");
            billingContractService.ServiceID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Service");
            billingContractService.StatusID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Status");
            billingContractService.ActivationDate = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("ActivationDate");
            billingContractService.SuspensionDate = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("SuspensionDate");
            billingContractService.EffectiveBillingAccountId = (long)genericBusinessEntity.FieldValues.GetRecord("EffectiveBillingAccount"); ;
            billingContractService.RatePlanId = (int)genericBusinessEntity.FieldValues.GetRecord("RatePlan");
            return billingContractService;
        }

    }
}
