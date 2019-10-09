using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Business
{
    public class InstallmentManager
    {
        static Guid s_BusinessEntityDefinitionId_Installment = new Guid("5d9f13bb-cd04-4a75-bd6c-a5ef5467cf15");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        public AddInstallmentOutput AddInstallment(AddInstallmentInput input)
        {
            var item = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Installment,
                FieldValues = new Dictionary<string, object>()
            };

            item.FieldValues.Add("BillingAccount", input.BillingAccountId);
            item.FieldValues.Add("InstallmentType", input.InstallmentTypeId);
            item.FieldValues.Add("ParentId", input.ParentId);
            item.FieldValues.Add("Currency", input.CurrencyId);
            item.FieldValues.Add("NbOfItems", input.NbOfItems);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(item);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Installment Failed while inserting the record into the Installment Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddInstallmentOutput
            {
                InstallmentId = itemId
            };
        }
    }
}
