using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
	public class LogCustomersToNotify : CodeActivity
	{
		#region Input Arguments

		[RequiredArgument]
		public InArgument<IEnumerable<int>> CustomerIdsToNotify { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IEnumerable<int> customerIdsToNotify = CustomerIdsToNotify.Get(context);

			if (customerIdsToNotify == null || customerIdsToNotify.Count() == 0)
				return;

			var customerNames = new List<string>();
			var carrierAccountManager = new CarrierAccountManager();

			foreach (int customerId in customerIdsToNotify)
			{
				string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
				if (customerName != null)
					customerNames.Add(customerName);
			}

			if (customerNames.Count == 0)
				return;

			string message = (customerNames.Count > 1) ? "Sending Sale Pricelists to Customers: {0}" : "Sending Sale Pricelist to Customer: {0}";
			context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, string.Join(",", customerNames));
		}
	}
}
