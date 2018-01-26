using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public sealed class SetRatePlanContext : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsAdditionalOwner { get; set; }

        #endregion

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.AddDefaultExtensionProvider<IRatePlanContext>(() => new RatePlanContext());
            base.CacheMetadata(metadata);
        }

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            bool isAdditionalOwner = IsAdditionalOwner.Get(context);
            PricingSettings pricingSettings = UtilitiesManager.GetPricingSettings(ownerType, ownerId);

            int currencyId = CurrencyId.Get(context);
            DateTime effectiveDate = EffectiveDate.Get(context);
            CurrencyManager currencyManager = new CurrencyManager();
            var systemCurrency = currencyManager.GetSystemCurrency();
            if (systemCurrency == null)
                throw new DataIntegrityValidationException("System Currency was not found");

            RatePlanContext ratePlanContext = context.GetRatePlanContext() as RatePlanContext;
            ratePlanContext.OwnerType = ownerType;
            ratePlanContext.OwnerId = ownerId;
            ratePlanContext.IsAdditionalOwner = isAdditionalOwner;
            ratePlanContext.OwnerSellingNumberPlanId = GetOwnerSellingNumberPlanId(ownerType, ownerId);
            ratePlanContext.CurrencyId = currencyId;
            ratePlanContext.setRatePlanContextPricingSettings(pricingSettings);

            int sellingProductId = (ownerType == SalePriceListOwnerType.SellingProduct) ? ownerId : new CarrierAccountManager().GetSellingProductId(ownerId);
            ratePlanContext.SellingProductCurrencyId = new SellingProductManager().GetSellingProductCurrencyId(sellingProductId);

            ratePlanContext.EffectiveDate = effectiveDate;
            ratePlanContext.RateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveDate));
            ratePlanContext.FutureRateLocator = GetFutureRateLocator(ownerType, ownerId, effectiveDate);
            ratePlanContext.PriceListCurrencyId = currencyId;
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                ratePlanContext.MinimumZoneBED = new SaleZoneManager().GetSaleZonesBySellingNumberPlan(ratePlanContext.OwnerSellingNumberPlanId).OrderBy(x => x.BED).First().BED;
                ratePlanContext.IsFirstSellingProductOffer = !new SalePriceListManager().CheckIfAnyPriceListExists(SalePriceListOwnerType.SellingProduct, ownerId);
            }

            if (isAdditionalOwner)
            {
                if (context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue)
                    ratePlanContext.RootProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value;
                else throw new InvalidWorkflowException("ParentProcessID Not Found");
            }
            else
                ratePlanContext.RootProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
        }

        #region Private Methods

        private int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingNumberPlanId;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(ownerId);
            }
            else
            {
                sellingNumberPlanId = new CarrierAccountManager().GetCustomerSellingNumberPlanId(ownerId);
            }

            if (!sellingNumberPlanId.HasValue)
            {
                string ownerTypeDescription = Vanrise.Common.Utilities.GetEnumDescription<SalePriceListOwnerType>(ownerType);
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Could not find the Selling Number Plan of {0} '{1}'", ownerTypeDescription, ownerId));
            }

            return sellingNumberPlanId.Value;
        }

        private SaleEntityZoneRateLocator GetFutureRateLocator(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveDate)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return null;

            int? sellingProductId = new CustomerSellingProductManager().GetEffectiveSellingProductId(ownerId, effectiveDate, false);
            if (!sellingProductId.HasValue)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' is not assigned to a selling product on '{1}'", ownerId, UtilitiesManager.GetDateTimeAsString(effectiveDate)));

            var dataByCustomer = new List<RoutingCustomerInfoDetails>();
            dataByCustomer.Add(new RoutingCustomerInfoDetails()
            {
                CustomerId = ownerId,
                SellingProductId = sellingProductId.Value
            });

            return new SaleEntityZoneRateLocator(new SaleRateReadAllNoCache(dataByCustomer, null, true));
        }

        #endregion
    }

    internal static class ContextExtensionMethods
    {
        public static IRatePlanContext GetRatePlanContext(this ActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            if (ratePlanContext == null)
                throw new NullReferenceException("ratePlanContext");
            return ratePlanContext;
        }
    }
}
