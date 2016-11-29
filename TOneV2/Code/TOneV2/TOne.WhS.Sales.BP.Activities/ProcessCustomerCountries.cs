using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	#region Classes

	public class ProcessCustomerCountriesInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }
		public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }
		public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
		public IEnumerable<ExistingCustomerCountry> ExistingCustomerCountries { get; set; }
	}

	public class ProcessCustomerCountriesOutput
	{
		public IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }
		public IEnumerable<ChangedCustomerCountry> ChangedCustomerCountries { get; set; }
		public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
	}

	#endregion

	public class ProcessCustomerCountries : Vanrise.BusinessProcess.BaseAsyncActivity<ProcessCustomerCountriesInput, ProcessCustomerCountriesOutput>
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }
		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }
		[RequiredArgument]
		public InArgument<IEnumerable<ExistingCustomerCountry>> ExistingCustomerCountries { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<NewCustomerCountry>> NewCustomerCountries { get; set; }
		
		[RequiredArgument]
		public OutArgument<IEnumerable<ChangedCustomerCountry>> ChangedCustomerCountries { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<ExistingCustomerCountry>> ExplicitlyChangedExistingCustomerCountries { get; set; }

		#endregion

		protected override ProcessCustomerCountriesInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new ProcessCustomerCountriesInput()
			{
				OwnerType = OwnerType.Get(context),
				CustomerCountriesToAdd = CustomerCountriesToAdd.Get(context),
				CustomerCountriesToChange = CustomerCountriesToChange.Get(context),
				ExistingCustomerCountries = ExistingCustomerCountries.Get(context)
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			if (NewCustomerCountries.Get(context) == null)
				NewCustomerCountries.Set(context, new List<NewCustomerCountry>());

			if (ChangedCustomerCountries.Get(context) == null)
				ChangedCustomerCountries.Set(context, new List<ChangedCustomerCountry>());

			if (ExplicitlyChangedExistingCustomerCountries.Get(context) == null)
				ExplicitlyChangedExistingCustomerCountries.Set(context, new List<ExistingCustomerCountry>());

			base.OnBeforeExecute(context, handle);
		}

		protected override ProcessCustomerCountriesOutput DoWorkWithResult(ProcessCustomerCountriesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			var processCustomerCountriesContext = new ProcessCustomerCountriesContext();

			if (inputArgument.OwnerType == SalePriceListOwnerType.Customer)
			{
				var plCustomerCountryManager = new PriceListCustomerCountryManager();

				processCustomerCountriesContext.CustomerCountriesToAdd = inputArgument.CustomerCountriesToAdd;
				processCustomerCountriesContext.CustomerCountriesToChange = inputArgument.CustomerCountriesToChange;
				processCustomerCountriesContext.ExistingCustomerCountries = inputArgument.ExistingCustomerCountries;
				
				plCustomerCountryManager.ProcessCustomerCountries(processCustomerCountriesContext);
			}
			else
			{
				processCustomerCountriesContext.ExplicitlyChangedExistingCustomerCountries = new List<ExistingCustomerCountry>();
			}

			return new ProcessCustomerCountriesOutput()
			{
				NewCustomerCountries = processCustomerCountriesContext.NewCustomerCountries,
				ChangedCustomerCountries = processCustomerCountriesContext.ChangedCustomerCountries,
				ExplicitlyChangedExistingCustomerCountries = processCustomerCountriesContext.ExplicitlyChangedExistingCustomerCountries
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessCustomerCountriesOutput result)
		{
			NewCustomerCountries.Set(context, result.NewCustomerCountries);
			ChangedCustomerCountries.Set(context, result.ChangedCustomerCountries);
			ExplicitlyChangedExistingCustomerCountries.Set(context, result.ExplicitlyChangedExistingCustomerCountries);
		}
	}
}
