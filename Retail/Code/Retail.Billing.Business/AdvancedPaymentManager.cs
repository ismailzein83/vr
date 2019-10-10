using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Business
{
    public class AdvancedPaymentManager
    {
        static Guid s_BusinessEntityDefinitionId_AdvancedPayment = new Guid("81ac1c85-a09c-41e2-95a3-9e6ddcbe933e");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        public AddAdvancedPaymentOutput AddAdvancedPayment(AddAdvancedPaymentInput input)
        {
            var item = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_AdvancedPayment,
                FieldValues = new Dictionary<string, object>()
            };

            item.FieldValues.Add("BillingAccount", input.BillingAccountId);
            item.FieldValues.Add("Contract", input.ContractId);
            item.FieldValues.Add("ContractService", input.ContractServiceId);
            item.FieldValues.Add("Amount", input.Amount);
            item.FieldValues.Add("Currency", input.CurrencyId);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(item);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add AdvancedPayment Failed while inserting the record into the AdvancedPayment Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddAdvancedPaymentOutput
            {
                AdvancedPaymentId = itemId
            };
        }
    }
}
