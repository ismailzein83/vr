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
        public int OwnerId { get; set; }
		public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }
		public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
	}

	public class PrepareCustomerCountryPreviewsOutput
	{
		public IEnumerable<NewCustomerCountryPreview> NewCustomerCountryPreviews { get; set; }
		public IEnumerable<ChangedCustomerCountryPreview> ChangedCustomerCountryPreviews { get; set; }
	}

	#endregion

	public class PrepareCustomerCountryPreviews : Vanrise.BusinessProcess.BaseAsyncActivity<PrepareCustomerCountryPreviewsInput, PrepareCustomerCountryPreviewsOutput>
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<NewCustomerCountryPreview>> NewCustomerCountryPreviews { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<ChangedCustomerCountryPreview>> ChangedCustomerCountryPreviews { get; set; }

		#endregion

		protected override PrepareCustomerCountryPreviewsInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new PrepareCustomerCountryPreviewsInput()
			{
				OwnerType = OwnerType.Get(context),
                OwnerId = OwnerId.Get(context),
				CustomerCountriesToAdd = CustomerCountriesToAdd.Get(context),
				CustomerCountriesToChange = CustomerCountriesToChange.Get(context)
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			if (ChangedCustomerCountryPreviews.Get(context) == null)
				ChangedCustomerCountryPreviews.Set(context, new List<ChangedCustomerCountryPreview>());
			if (NewCustomerCountryPreviews.Get(context) == null)
				NewCustomerCountryPreviews.Set(context, new List<NewCustomerCountryPreview>());
			base.OnBeforeExecute(context, handle);
		}

		protected override PrepareCustomerCountryPreviewsOutput DoWorkWithResult(PrepareCustomerCountryPreviewsInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
		{
			SalePriceListOwnerType ownerType = inputArgument.OwnerType;
			IEnumerable<CustomerCountryToAdd> customerCountriesToAdd = inputArgument.CustomerCountriesToAdd;
			IEnumerable<CustomerCountryToChange> customerCountriesToChange = inputArgument.CustomerCountriesToChange;
            int ownerId = inputArgument.OwnerId;
			var newCustomerCountryPreviews = new List<NewCustomerCountryPreview>();
			var changedCustomerCountryPreviews = new List<ChangedCustomerCountryPreview>();

			if (ownerType == SalePriceListOwnerType.Customer)
			{
				foreach (CustomerCountryToAdd countryToAdd in customerCountriesToAdd)
				{
					newCustomerCountryPreviews.Add(new NewCustomerCountryPreview()
					{
						CountryId = countryToAdd.CountryId,
                        CustomerId = ownerId,
						BED = countryToAdd.BED,
						EED = countryToAdd.EED
					});
				}
				foreach (CustomerCountryToChange countryToChange in customerCountriesToChange)
				{
					changedCustomerCountryPreviews.Add(new ChangedCustomerCountryPreview()
					{
						CountryId = countryToChange.CountryId,
                        CustomerId = ownerId,
						EED = countryToChange.CloseEffectiveDate
					});
				}
			}

			return new PrepareCustomerCountryPreviewsOutput()
			{
				NewCustomerCountryPreviews = newCustomerCountryPreviews,
				ChangedCustomerCountryPreviews = changedCustomerCountryPreviews
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareCustomerCountryPreviewsOutput result)
		{
			NewCustomerCountryPreviews.Set(context, result.NewCustomerCountryPreviews);
			ChangedCustomerCountryPreviews.Set(context, result.ChangedCustomerCountryPreviews);
		}
	}
}
