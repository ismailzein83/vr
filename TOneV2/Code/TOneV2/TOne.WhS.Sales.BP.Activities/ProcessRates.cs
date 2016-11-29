﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	#region Classes

	public class ProcessRatesInput
	{
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
	}
	
	#endregion

    public class ProcessRates : BaseAsyncActivity<ProcessRatesInput, ProcessRatesOutput>
    {
        #region Input Arguments

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

        #endregion

        protected override ProcessRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessRatesInput()
            {
                RatesToChange = this.RatesToChange.Get(context),
                RatesToClose = this.RatesToClose.Get(context),
                ExistingZones = this.ExistingZones.Get(context),
                ExistingRates = this.ExistingRates.Get(context),
				ExplicitlyChangedExistingCustomerCountries = ExplicitlyChangedExistingCustomerCountries.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.NewRates.Get(context) == null)
                this.NewRates.Set(context, new List<NewRate>());

            if (this.ChangedRates.Get(context) == null)
                this.ChangedRates.Set(context, new List<ChangedRate>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessRatesOutput DoWorkWithResult(ProcessRatesInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<RateToChange> ratesToChange = inputArgument.RatesToChange;
            IEnumerable<RateToClose> ratesToClose = inputArgument.RatesToClose;
            IEnumerable<ExistingZone> existingZones = inputArgument.ExistingZones;
            IEnumerable<ExistingRate> existingRates = inputArgument.ExistingRates;
			IEnumerable<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries = inputArgument.ExplicitlyChangedExistingCustomerCountries;

            var priceListRateManager = new PriceListRateManager();

            var processRatesContext = new ProcessRatesContext()
            {
                RatesToChange = ratesToChange,
                RatesToClose = ratesToClose,
                ExistingZones = existingZones,
                ExistingRates = existingRates,
				ExplicitlyChangedExistingCustomerCountries = explicitlyChangedExistingCustomerCountries
            };

            priceListRateManager.ProcessCountryRates(processRatesContext);

            return new ProcessRatesOutput()
            {
                NewRates = processRatesContext.NewRates,
                ChangedRates = processRatesContext.ChangedRates
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessRatesOutput result)
        {
            this.NewRates.Set(context, result.NewRates);
            this.ChangedRates.Set(context, result.ChangedRates);
        }
    }
}
