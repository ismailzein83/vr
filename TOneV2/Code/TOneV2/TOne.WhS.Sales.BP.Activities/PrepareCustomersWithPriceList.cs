using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public class PrepareCustomersWithPriceList : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<int>> CustomerIdsWithPriceList { get; set; }

		#endregion

		#region Output Arguments

		[RequiredArgument]
		public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersWithPriceList { get; set; }

		#endregion

		protected override void Execute(System.Activities.CodeActivityContext context)
		{
			IEnumerable<int> customerIdsWithPriceList = CustomerIdsWithPriceList.Get(context);

			var customersWithPriceList = new List<CarrierAccountInfo>();
			var carrierAccountManager = new CarrierAccountManager();

			foreach (int customerId in customerIdsWithPriceList)
			{
				CarrierAccountInfo customerInfo = CustomerInfoMapper(customerId, carrierAccountManager);
				customersWithPriceList.Add(customerInfo);
			}

			CustomersWithPriceList.Set(context, customersWithPriceList);
		}

		#region Private Methods

		private CarrierAccountInfo CustomerInfoMapper(int customerId, CarrierAccountManager carrierAccountManager)
		{
			CarrierAccount customer = carrierAccountManager.GetCarrierAccount(customerId);
			if (customer == null)
				throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' was not found", customerId));
			else
			{
				return new CarrierAccountInfo()
				{
					CarrierAccountId = customerId,
					Name = carrierAccountManager.GetCarrierAccountName(customerId),
					AccountType = customer.AccountType,
					SellingNumberPlanId = customer.SellingNumberPlanId
				};
			}
		}
		
		#endregion
	}
}
