using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using SOM.Main.BP.Arguments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;
using Terrasoft.Core.Entities;

namespace BPMExtended.Main.Business
{
    public class BillingManager
    {
        public CustomerBalance GetCustomerBalance(string customerId)
        {
            return RatePlanMockDataGenerator.GetCustomerBalance(customerId);
        }

        public PaymentInfo SubmitToPOS(string customerId, string requestId, string ratePlanId, Guid contactId)
        {
            //After creating a contract with status on hold for this customer
            //Send to POS the list of services to pay with the contract id

            decimal depositAmount =0;
            bool isForeigner = false;
            PaymentInfo payment = new PaymentInfo();

            RatePlanManager ratePlanManager = new RatePlanManager();
            var coreServices = ratePlanManager.GetCoreServices(ratePlanId);

            decimal amountToPay = 0;

            foreach (var service in coreServices)
            {
                amountToPay += service.SubscriptionFee;
            }
          
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var esqResult = new EntitySchemaQuery(connection.EntitySchemaManager, "Contact");
            esqResult.AddColumn("Name");
            esqResult.AddColumn("StCustomerDocumentType");
            esqResult.AddColumn("StSponsorDocumentIDNumber");

            // Execution of query to database and getting object with set identifier.
            var entity = esqResult.GetEntity(connection, contactId);
            object customerTypeId = entity.GetColumnValue("StCustomerDocumentTypeId");
            object sponsorNumber = entity.GetColumnValue("StSponsorDocumentIDNumber");

            //get customer type
            var esqResult2 = new EntitySchemaQuery(connection.EntitySchemaManager, "StCustomerDocumentType");
            esqResult2.AddColumn("Name");
            var entity2 = esqResult2.GetEntity(connection, customerTypeId);
            object customerType = entity2.GetColumnValue("Name");


            //get services
            var esqResult3 = new EntitySchemaQuery(connection.EntitySchemaManager, "StLineSubscriptionRequest");
            esqResult3.AddColumn("StServices");
            var entity3 = esqResult3.GetEntity(connection, requestId);
            object servicesJson = entity3.GetColumnValue("StServices");

            List<Service> services = JsonConvert.DeserializeObject<List<Service>>(servicesJson.ToString());


            if (customerType.Equals("أجنبي") && sponsorNumber !=null && !sponsorNumber.ToString().Equals(""))
            {
                depositAmount = 15000;
                isForeigner = true;
            }


            //
            payment.amountToPay = amountToPay;
            payment.isForeigner = isForeigner;
            payment.depositAmount = depositAmount;

            return payment;
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

        public bool isUserNameUnique(string userName)
        {
            //TODO: call BSCS to validate the username
            return true;
        }

        
    }

    public class PaymentInfo
    {

        public decimal amountToPay { get; set; }
        public bool isForeigner { get; set; }
        public decimal depositAmount { get; set; }


    }

    public class Service
    {

        public string Id { get; set; }

    }
}
