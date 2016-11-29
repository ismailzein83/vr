using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	#region Classes

	public class GetExistingCustomerCountriesInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }

		public int OwnerId { get; set; }

		public DateTime MinimumDate { get; set; }
	}

	public class GetExistingCustomerCountriesOutput
	{
		public IEnumerable<CustomerCountry2> ExistingSaleCustomerCountries { get; set; }
	}

	#endregion

	public class GetExistingCustomerCountries : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingCustomerCountriesInput, GetExistingCustomerCountriesOutput>
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public InArgument<DateTime> MinimumDate { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<CustomerCountry2>> ExistingSaleCustomerCountries { get; set; }

		#endregion

		protected override GetExistingCustomerCountriesInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new GetExistingCustomerCountriesInput()
			{
				OwnerType = OwnerType.Get(context),
				OwnerId = OwnerId.Get(context),
				MinimumDate = MinimumDate.Get(context)
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
		{
			if (ExistingSaleCustomerCountries.Get(context) == null)
				ExistingSaleCustomerCountries.Set(context, new List<CustomerCountry2>());
			base.OnBeforeExecute(context, handle);
		}

		protected override GetExistingCustomerCountriesOutput DoWorkWithResult(GetExistingCustomerCountriesInput inputArgument, AsyncActivityHandle handle)
		{
			SalePriceListOwnerType ownerType = inputArgument.OwnerType;
			int ownerId = inputArgument.OwnerId;
			DateTime minimumDate = inputArgument.MinimumDate;

			IEnumerable<CustomerCountry2> customerCountries = null;

			if (ownerType == SalePriceListOwnerType.Customer)
			{
				var customerCountryManager = new CustomerCountryManager();
				customerCountries = customerCountryManager.GetCustomerCountriesEffectiveAfter(ownerId, minimumDate);
			}

			return new GetExistingCustomerCountriesOutput
			{
				ExistingSaleCustomerCountries = customerCountries
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingCustomerCountriesOutput result)
		{
			ExistingSaleCustomerCountries.Set(context, result.ExistingSaleCustomerCountries);
		}
	}
}
