using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Billing.Business
{
    public class ContractServiceActionManager
    {
        static Guid s_BusinessEntityDefinitionId_ContractServiceAction = new Guid("fe15c487-b948-4f22-82e3-6c50c608c51d");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        static ContractServiceManager s_contractServiceManager = new ContractServiceManager();
        static RatePlanManager s_ratePlanManager = new RatePlanManager();


        #region Public Methods

        public AddContractServiceActionOutput AddContractServiceAction(AddContractServiceActionInput input)
        {
            var itemToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractServiceAction,
                FieldValues = new System.Collections.Generic.Dictionary<string, object>()
            };

            itemToAdd.FieldValues.Add("ContractService", input.ContractServiceId);
            itemToAdd.FieldValues.Add("ActionType", input.ActionTypeId);

            var chargeTime = input.ChargeTime.HasValue ? input.ChargeTime.Value : DateTime.Now;
            itemToAdd.FieldValues.Add("ChargeTime", chargeTime);

            itemToAdd.FieldValues.Add("PaidCash", input.PaidCash);

            itemToAdd.FieldValues.Add("Charge", CalculateActionCharge(input));

            if (input.OverriddenCharge.HasValue)
                itemToAdd.FieldValues.Add("OverriddenCharge", input.OverriddenCharge);

            var actionCharge = CalculateActionCharge(input);

            if (input.OldServiceOptionId.HasValue)
                itemToAdd.FieldValues.Add("OldServiceOption", input.OldServiceOptionId.Value);

            if (input.NewServiceOptionId.HasValue)
                itemToAdd.FieldValues.Add("NewServiceOption", input.NewServiceOptionId.Value);

            if (input.OldServiceOptionActivationFee.HasValue)
                itemToAdd.FieldValues.Add("OldServiceOptionActivationFee", input.OldServiceOptionActivationFee.Value);

            if (input.NewServiceOptionActivationFee.HasValue)
                itemToAdd.FieldValues.Add("NewServiceOptionActivationFee", input.NewServiceOptionActivationFee.Value);

            if (input.OldSpeedInMbps.HasValue)
                itemToAdd.FieldValues.Add("OldSpeedInMbps", input.OldSpeedInMbps.Value);

            if (input.NewSpeedInMbps.HasValue)
                itemToAdd.FieldValues.Add("NewSpeedInMbps", input.NewSpeedInMbps.Value);

            var insertOutput = s_genericBEManager.AddGenericBusinessEntity(itemToAdd);

            if (insertOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Contract Service Action Failed while inserting the record into the ContractServiceAction Table. Result is: '{insertOutput.Result.ToString()}'. Error Message: '{insertOutput.Message}'");

            var itemId = (long)insertOutput.InsertedObject.FieldValues["ID"].Value;

            return new AddContractServiceActionOutput
            {
                ServiceContractActionId = itemId
            };
        }

        private decimal CalculateActionCharge(AddContractServiceActionInput input)
        {
            var contractService = s_contractServiceManager.GetContractService(input.ContractServiceId);

            contractService.ThrowIfNull("contractService", input.ContractServiceId);

            var evaluateActionChargeInput = new RatePlanServiceActionChargeEvaluationInput
            {
                RatePlanId = contractService.RatePlanId,
                ContractServiceAction = new ChargeTargetContractServiceAction
                {
                    ActionType = input.ActionTypeId,
                    ContractService = input.ContractServiceId,
                    ServiceType = contractService.ServiceTypeId,
                    ServiceTypeOption = contractService.ServiceTypeOptionId,
                    OldServiceOption = input.OldServiceOptionId,
                    NewServiceOption = input.NewServiceOptionId,
                    OldServiceOptionActivationFee = input.OldServiceOptionActivationFee,
                    NewServiceOptionActivationFee = input.NewServiceOptionActivationFee,
                    OldSpeedInMbps = input.OldSpeedInMbps,
                    NewSpeedInMbps = input.NewSpeedInMbps
                }
            };
            var evaluationOutput = s_ratePlanManager.EvaluateActionCharge(evaluateActionChargeInput);
            return evaluationOutput != null ? evaluationOutput.Charge : 0;
        }

        #endregion
    }
}
