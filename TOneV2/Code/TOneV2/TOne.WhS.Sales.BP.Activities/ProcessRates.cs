using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ProcessRatesInput
    {
        public int? ReservedOwnerPriceListId { get; set; }

        public IEnumerable<RateToChange> RatesToChange { get; set; }

        public IEnumerable<RateToClose> RatesToClose { get; set; }

        public IEnumerable<ExistingZone> ExistingZones { get; set; }

        public IEnumerable<ExistingRate> ExistingRates { get; set; }

        public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
    }

    public class ProcessRatesOutput
    {
        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<ChangedRate> ChangedRates { get; set; }

        public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }
    }

    #endregion

    public class ProcessRates : BaseAsyncActivity<ProcessRatesInput, ProcessRatesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int?> ReservedOwnerPriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingRate>> ExistingRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCustomerCountry>> ExplicitlyChangedExistingCustomerCountries { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<int, List<NewPriceList>>> CustomerPriceListsByCurrencyId { get; set; }

        #endregion

        protected override ProcessRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessRatesInput()
            {
                ReservedOwnerPriceListId = ReservedOwnerPriceListId.Get(context),
                RatesToChange = this.RatesToChange.Get(context),
                RatesToClose = this.RatesToClose.Get(context),
                ExistingZones = this.ExistingZones.Get(context),
                ExistingRates = this.ExistingRates.Get(context),
                ExplicitlyChangedExistingCustomerCountries = ExplicitlyChangedExistingCustomerCountries.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);

            if (this.NewRates.Get(context) == null)
                this.NewRates.Set(context, new List<NewRate>());

            if (this.ChangedRates.Get(context) == null)
                this.ChangedRates.Set(context, new List<ChangedRate>());

            if (CustomerPriceListsByCurrencyId.Get(context) == null)
                CustomerPriceListsByCurrencyId.Set(context, new Dictionary<int, List<NewPriceList>>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessRatesOutput DoWorkWithResult(ProcessRatesInput inputArgument, AsyncActivityHandle handle)
        {
            RatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as RatePlanContext;

            int? reservedOwnerPriceListId = inputArgument.ReservedOwnerPriceListId;
            IEnumerable<RateToChange> ratesToChange = inputArgument.RatesToChange;
            IEnumerable<RateToClose> ratesToClose = inputArgument.RatesToClose;
            IEnumerable<ExistingZone> existingZones = inputArgument.ExistingZones;
            IEnumerable<ExistingRate> existingRates = inputArgument.ExistingRates;
            IEnumerable<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries = inputArgument.ExplicitlyChangedExistingCustomerCountries;

            var priceListRateManager = new PriceListRateManager();

            var processRatesContext = new ProcessRatesContext()
            {
                ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID,
                UserId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId,
                OwnerType = ratePlanContext.OwnerType,
                OwnerId = ratePlanContext.OwnerId,
                PriceListCreationDate = ratePlanContext.PriceListCreationDate,
                CurrencyId = ratePlanContext.CurrencyId,
                LongPrecisionValue = ratePlanContext.LongPrecision,
                RatesToChange = ratesToChange,
                RatesToClose = ratesToClose,
                ExistingZones = existingZones,
                ExistingRates = existingRates,
                ExplicitlyChangedExistingCustomerCountries = explicitlyChangedExistingCustomerCountries,
                InheritedRatesByZoneId = ratePlanContext.InheritedRatesByZoneId,
                ReservedPriceListId = (reservedOwnerPriceListId.HasValue) ? reservedOwnerPriceListId.Value : 0
            };

            priceListRateManager.ProcessCountryRates(processRatesContext);

            if (DoRateChangesExist(processRatesContext))
            {
                ratePlanContext.SetProcessHasChangesToTrueWithLock();
            }

            return new ProcessRatesOutput()
            {
                NewRates = processRatesContext.NewRates,
                ChangedRates = processRatesContext.ChangedRates,
                CustomerPriceListsByCurrencyId = processRatesContext.CustomerPriceListsByCurrencyId
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessRatesOutput result)
        {
            this.NewRates.Set(context, result.NewRates);
            this.ChangedRates.Set(context, result.ChangedRates);
            CustomerPriceListsByCurrencyId.Set(context, result.CustomerPriceListsByCurrencyId);
        }

        #region Private Methods

        private bool DoRateChangesExist(ProcessRatesContext context)
        {
            if (context.NewRates != null && context.NewRates.Count() > 0)
                return true;

            if (context.ChangedRates != null && context.ChangedRates.Count() > 0)
                return true;

            return false;
        }

        #endregion
    }
}
