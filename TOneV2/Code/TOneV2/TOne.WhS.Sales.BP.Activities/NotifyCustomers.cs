using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	public class NotifyCustomers : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<int>> CustomerIds { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			int ownerId = OwnerId.Get(context);
			SalePriceListOwnerType ownerType = OwnerType.Get(context);
			IEnumerable<SalePLZoneChange> salePLZoneChanges = SalePLZoneChanges.Get(context);
			IEnumerable<int> customerIds = CustomerIds.Get(context);

			if (customerIds == null || customerIds.Count() == 0)
				throw new NullReferenceException("customerIds");

			int sellingNumberPlanId = (ownerType == SalePriceListOwnerType.SellingProduct) ?
				GetSellingProductSellingNumberPlanId(ownerId) :
				GetCustomerSellingNumberPlanId(ownerId);

			var notificationContext = new NotificationContext()
			{
				SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
				CustomerIds = customerIds,
				ZoneChanges = salePLZoneChanges,
				EffectiveDate = DateTime.Today,
				ChangeType = SalePLChangeType.Rate,
				InitiatorId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId
			};

			NotificationManager notificationManager = new NotificationManager();
			notificationManager.BuildNotifications(notificationContext);
		}

		#region Private Methods

		private int GetSellingProductSellingNumberPlanId(int sellingProductId)
		{
			var sellingProductManager = new SellingProductManager();
			int? sellingNumberPlanId = sellingProductManager.GetSellingNumberPlanId(sellingProductId);
			if (!sellingNumberPlanId.HasValue)
				throw new NullReferenceException(string.Format("SellingProduct '{0}' was not found", sellingProductId));
			return sellingNumberPlanId.Value;
		}

		private int GetCustomerSellingNumberPlanId(int customerId)
		{
			var carrierAccountManager = new CarrierAccountManager();
			CarrierAccount customerAccount = carrierAccountManager.GetCarrierAccount(customerId);
			if (customerAccount == null)
				throw new NullReferenceException(string.Format("Customer '{0}' was not found", customerId));
			if (customerAccount.AccountType == CarrierAccountType.Supplier)
				throw new Exception(string.Format("CarrierAccount '{0}' is not a Customer", customerId));
			return customerAccount.SellingNumberPlanId.Value;
		}
		
		#endregion
	}
}
