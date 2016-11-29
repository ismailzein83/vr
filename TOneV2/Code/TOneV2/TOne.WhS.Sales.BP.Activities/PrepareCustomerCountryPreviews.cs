using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	#region Classes

	public class PrepareCustomerCountryPreviewsInput
	{
		public SalePriceListOwnerType OwnerType { get; set; }
		public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
	}

	public class PrepareCustomerCountryPreviewsOutput
	{
		public IEnumerable<ChangedCustomerCountryPreview> ChangedCustomerCountryPreviews { get; set; }
	}

	#endregion

	public class PrepareCustomerCountryPreviews : Vanrise.BusinessProcess.BaseAsyncActivity<PrepareCustomerCountryPreviewsInput, PrepareCustomerCountryPreviewsOutput>
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<ChangedCustomerCountryPreview>> ChangedCustomerCountryPreviews { get; set; }

		#endregion

		protected override PrepareCustomerCountryPreviewsInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new PrepareCustomerCountryPreviewsInput()
			{
				OwnerType = OwnerType.Get(context),
				CustomerCountriesToChange = CustomerCountriesToChange.Get(context)
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			if (ChangedCustomerCountryPreviews.Get(context) == null)
				ChangedCustomerCountryPreviews.Set(context, new List<ChangedCustomerCountryPreview>());
			base.OnBeforeExecute(context, handle);
		}

		protected override PrepareCustomerCountryPreviewsOutput DoWorkWithResult(PrepareCustomerCountryPreviewsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			SalePriceListOwnerType ownerType = inputArgument.OwnerType;
			IEnumerable<CustomerCountryToChange> customerCountriesToChange = inputArgument.CustomerCountriesToChange;

			var changedCustomerCountryPreviews = new List<ChangedCustomerCountryPreview>();

			if (ownerType == SalePriceListOwnerType.Customer)
			{
				foreach (CustomerCountryToChange countryToChange in customerCountriesToChange)
				{
					changedCustomerCountryPreviews.Add(new ChangedCustomerCountryPreview()
					{
						CountryId = countryToChange.CountryId,
						EED = countryToChange.CloseEffectiveDate
					});
				}
			}

			return new PrepareCustomerCountryPreviewsOutput()
			{
				ChangedCustomerCountryPreviews = changedCustomerCountryPreviews
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareCustomerCountryPreviewsOutput result)
		{
			ChangedCustomerCountryPreviews.Set(context, result.ChangedCustomerCountryPreviews);
		}
	}
}
