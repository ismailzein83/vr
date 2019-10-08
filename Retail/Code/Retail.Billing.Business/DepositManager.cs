using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Business
{
    public class DepositManager
    {
        static Guid s_BusinessEntityDefinitionId_Deposit = new Guid("ef7ded63-ee03-439a-8397-07333cda6115");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        public AddDepositOutput AddDeposit(AddDepositInput input)
        {
            var item = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Deposit,
                FieldValues = new Dictionary<string, object>()
            };

            item.FieldValues.Add("BillingAccount", input.BillingAccountId);
            item.FieldValues.Add("Contract", input.ContractId);
            item.FieldValues.Add("ContractService", input.ContractServiceId);
            item.FieldValues.Add("DepositAmount", input.Amount);
            item.FieldValues.Add("Currency", input.CurrencyId);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(item);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Deposit Failed while inserting the record into the Deposit Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddDepositOutput
            {
                DepositId = itemId
            };
        }

    }
}
