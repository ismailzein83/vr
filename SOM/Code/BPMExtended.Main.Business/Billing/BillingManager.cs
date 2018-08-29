using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.BP.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class BillingManager
    {
        public CustomerBalance GetCustomerBalance(string customerId)
        {
            return RatePlanMockDataGenerator.GetCustomerBalance(customerId);
        }

        public decimal SubmitToPOS(string customerId, string requestId, string ratePlanId)
        {
            //After creating a contract with status on hold for this customer
            //Send to POS the list of services to pay with the contract id

            RatePlanManager ratePlanManager = new RatePlanManager();
            var coreServices = ratePlanManager.GetCoreServices(ratePlanId);

            decimal amountToPay = 0;

            foreach (var service in coreServices)
            {
                amountToPay += service.SubscriptionFee;
            }

            return amountToPay;
        }

        public decimal SubmitToPOS(string contractId, string requestId, OperationType operationType)
        {
            //Get from BPM the list of core services mapped to this operation type
            //Send to POS the list of services to pay with the contract id

            return 3500;
        }

        public bool ValidatePayment(string requestId)
        {
            return true;
        }

        
    }
}
