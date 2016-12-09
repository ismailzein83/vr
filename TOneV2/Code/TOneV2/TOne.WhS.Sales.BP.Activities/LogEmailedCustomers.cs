using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
	public class LogEmailedCustomers : CodeActivity
	{
		#region Input Arguments
		
		[RequiredArgument]
		public InArgument<IEnumerable<int>> CustomerIdsToEmail { get; set; }
		
		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			IEnumerable<int> customerIdsToEmail = CustomerIdsToEmail.Get(context);

			if (customerIdsToEmail == null || customerIdsToEmail.Count() == 0)
				throw new Vanrise.Entities.MissingArgumentValidationException("Failed to notify Customers: No Customers were selected");

			var customerNames = new List<string>();
			var carrierAccountManager = new CarrierAccountManager();

			foreach (int customerId in customerIdsToEmail)
			{
				string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
				if (customerName == null)
					throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of Customer '{0}' was not found", customerId));
				customerNames.Add(customerName);
			}

			string message = (customerNames.Count > 1) ? "Sale Pricelists have been emailed to Customers: {0}" : "Sale Pricelist has been emailed to Customer: {0}";
			context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, string.Join(",", customerNames));
		}
	}
}
