using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Business
{
    public class ContractResourceManager
    {
        static Guid s_BusinessEntityDefinitionId_ContractResource = new Guid("1754de8b-c2d8-446d-be09-d4196c224a66");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        public AddContractResourceOutput AddContractResource(AddContractResourceInput input)
        {
            var item = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractResource,
                FieldValues = new Dictionary<string, object>()
            };

            item.FieldValues.Add("Contract", input.ContractId);
            item.FieldValues.Add("Name", input.Name);
            item.FieldValues.Add("Type", input.ResourceTypeId);
            item.FieldValues.Add("BET", input.BET);
            if (input.EET.HasValue)
                item.FieldValues.Add("EET", input.EET.Value);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(item);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Contract Resource Failed while inserting the record into the ContractResource Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddContractResourceOutput
            {
                ContractResourceId = itemId
            };
        }

        public CloseContractResourceOutput CloseContractResource(CloseContractResourceInput input)
        {
            var itemToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractResource,
                GenericBusinessEntityId = input.ContractResourceId,
                FieldValues = new Dictionary<string, object>()
            };

            itemToUpdate.FieldValues.Add("EET", input.CloseTime);

            s_genericBEManager.UpdateGenericBusinessEntity(itemToUpdate);

            return new CloseContractResourceOutput();
        }
    }
}
