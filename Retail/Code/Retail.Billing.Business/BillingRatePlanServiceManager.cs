using Retail.Billing.Entities;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class BillingRatePlanServiceManager
    {
        static readonly Guid BeDefinitionId = new Guid("435bba40-1865-413c-9af3-21c0555d9cef");

        #region Public 

        public BillingRatePlanService GetBillingRatePlanServiceByRatePlanAndService(long ratePlanId, Guid ServiceId)
        {
            var cachedBillingRatePlanServiceByRatePlanService = GetCachedBillingRatePlanServiceByRatePlanService();
            var billingRatePlanServiceByService = cachedBillingRatePlanServiceByRatePlanService.GetRecord(ratePlanId);

            if (billingRatePlanServiceByService != null)
                return billingRatePlanServiceByService.GetRecord(ServiceId);
            return null;
        }
        #endregion

        #region private 
        private Dictionary<long, Dictionary<Guid, BillingRatePlanService>> GetCachedBillingRatePlanServiceByRatePlanService()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedBillingRatePlanServiceByRatePlanService", BeDefinitionId, () =>
            {
                var billingRatePlanServices = GetAllBillingRatePlanServices();

                var billingRatePlanServiceByRatePlanService = new Dictionary<long, Dictionary<Guid, BillingRatePlanService>>();

                if (billingRatePlanServices != null)
                {
                    foreach (var item in billingRatePlanServices)
                    {
                        var billingRatePlanServiceByRatePlan = billingRatePlanServiceByRatePlanService.GetOrCreateItem(item.RatePlanId);
                        billingRatePlanServiceByRatePlan.Add(item.ServiceID, item);
                    }
                }
                return billingRatePlanServiceByRatePlanService;
            });
        }

        private IEnumerable<BillingRatePlanService> GetAllBillingRatePlanServices(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;
            List<BillingRatePlanService> financialAccounts = new List<BillingRatePlanService>();

            return entities.MapRecords(BillingContractServiceMapper);
        }

        private BillingRatePlanService BillingContractServiceMapper(GenericBusinessEntity genericBusinessEntity)
        {
            BillingRatePlanService billingRatePlanService = new BillingRatePlanService();
            billingRatePlanService.ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID");
            billingRatePlanService.ServiceID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Service");
            billingRatePlanService.RatePlanId = (int)genericBusinessEntity.FieldValues.GetRecord("RatePlan");

            var activationFee = genericBusinessEntity.FieldValues.GetRecord("ActivationFee");
            if (activationFee != null)
                billingRatePlanService.ActivationFee = activationFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", activationFee);

            var recurringFee = genericBusinessEntity.FieldValues.GetRecord("RecurringFee");
            if (recurringFee != null)
                billingRatePlanService.RecurringFee = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", recurringFee);

            var suspensionCharge = genericBusinessEntity.FieldValues.GetRecord("SuspensionCharge");
            if (suspensionCharge != null)
                billingRatePlanService.SuspensionCharge = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", suspensionCharge);

            var suspensionRecurringCharge = genericBusinessEntity.FieldValues.GetRecord("SuspensionRecurringCharge");
            if (suspensionRecurringCharge != null)
                billingRatePlanService.SuspensionRecurringCharge = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", suspensionRecurringCharge);

            var deactivationCharge = genericBusinessEntity.FieldValues.GetRecord("DeactivationCharge");
            if (deactivationCharge != null)
                billingRatePlanService.DeactivationCharge = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", deactivationCharge);

            var deposit = genericBusinessEntity.FieldValues.GetRecord("SuspensionRecurringCharge");
            if (deposit != null)
                billingRatePlanService.Deposit = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", deposit);

            var bankGuarantee = genericBusinessEntity.FieldValues.GetRecord("SuspensionRecurringCharge");
            if (bankGuarantee != null)
                billingRatePlanService.BankGuarantee = recurringFee.CastWithValidate<RetailBEChargeEntity>("RetailBEChargeEntity", bankGuarantee);
            return billingRatePlanService;
        }

        #endregion
    }
}
