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
				throw new Vanrise.Entities.MissingArgumentValidationException("Failed to notify Customers: No Customers were selected");

			var customerNames = new List<string>();
			var carrierAccountManager = new CarrierAccountManager();

			foreach (int customerId in customerIdsToNotify)
			{
				string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
				if (customerName == null)
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of Customer '{0}' was not found", customerId));
				customerNames.Add(customerName);
			}

			string message = (customerNames.Count > 1) ? "Sending Sale Pricelists to Customers: {0}" : "Sending Sale Pricelist to Customer: {0}";
			context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, string.Join(",", customerNames));
		}
	}
}
