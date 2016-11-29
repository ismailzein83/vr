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
	public class PrepareExistingCustomerCountries : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountry2>> ExistingSaleCustomerCountries { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<ExistingCustomerCountry>> ExistingCustomerCountries { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			SalePriceListOwnerType ownerType = OwnerType.Get(context);
			IEnumerable<CustomerCountry2> existingSaleCustomerCountries = ExistingSaleCustomerCountries.Get(context);

			var existingCustomerCountries = new List<ExistingCustomerCountry>();

			if (ownerType == SalePriceListOwnerType.Customer)
			{
				if (existingSaleCustomerCountries != null)
				{
					foreach (CustomerCountry2 customerCountry in existingSaleCustomerCountries)
					{
						existingCustomerCountries.Add(new ExistingCustomerCountry()
						{
							CustomerCountryEntity = customerCountry
						});
					}
				}
			}

			ExistingCustomerCountries.Set(context, existingCustomerCountries);
		}
	}
}
