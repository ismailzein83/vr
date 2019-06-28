using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
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

            return entities.MapRecords(BillingContractServiceMapper);
        }

        private BillingContractService BillingContractServiceMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new BillingContractService()
            {
                ID = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                ContractID = (long)genericBusinessEntity.FieldValues.GetRecord("Contract"),
                ServiceID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Service"),
                StatusID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Status"),
                ActivationDate = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("ActivationDate"),
                SuspensionDate = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("SuspensionDate"),
                EffectiveBillingAccountId = (long)genericBusinessEntity.FieldValues.GetRecord("EffectiveBillingAccount"),
                RatePlanId = (int)genericBusinessEntity.FieldValues.GetRecord("RatePlan")
            };
        }

    }
}
