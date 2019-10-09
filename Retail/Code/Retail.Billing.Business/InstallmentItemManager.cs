using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Business
{
    public class InstallmentItemManager
    {
        static Guid s_BusinessEntityDefinitionId_Installment = new Guid("404d3012-d233-4a91-ab82-d5c0f4d31062");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        public AddInstallmentItemOutput AddInstallmentItem(AddInstallmentItemInput input)
        {
            var item = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Installment,
                FieldValues = new Dictionary<string, object>()
            };

            item.FieldValues.Add("Installment", input.InstallmentId);
            item.FieldValues.Add("ItemNumber", input.ItemNumber);
            item.FieldValues.Add("Amount", input.Amount);
            item.FieldValues.Add("ScheduleDate", input.ScheduleDate);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(item);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Installment Item Failed while inserting the record into the InstallmentItem Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddInstallmentItemOutput
            {
                InstallmentItemId = itemId
            };
        }
    }
}
