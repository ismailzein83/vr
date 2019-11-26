using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class ContractServiceActionManager
    {
        static Guid s_BusinessEntityDefinitionId_ContractServiceAction = new Guid("fe15c487-b948-4f22-82e3-6c50c608c51d");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        static ContractServiceManager s_contractServiceManager = new ContractServiceManager();
        static RatePlanManager s_ratePlanManager = new RatePlanManager();


        #region Public Methods

        public List<ContractServiceAction> GetContractServiceActions(RecordFilterGroup filterGroup, List<string> columnsNeeded = null)
        {
            return s_genericBEManager.GetAllGenericBusinessEntities(s_BusinessEntityDefinitionId_ContractServiceAction, columnsNeeded, filterGroup).MapRecords(ContractServiceActionMapper).ToList();
        }
        public List<ContractServiceAction> GetContractServiceActionsByContractIds(List<long> contractIds, DateTime fromTime, DateTime toTime)
        {
            var filterGroup = new RecordFilterGroup { Filters = new List<RecordFilter>() };
            filterGroup.Filters.Add(new ObjectListRecordFilter()
            {
                FieldName = "Contract",
                Values = contractIds.MapRecords(x => (object)x).ToList()
            });

            var orFilterGroup = new RecordFilterGroup
            {
                Filters = new List<RecordFilter>(),
                LogicalOperator = RecordQueryLogicalOperator.Or
            };
            orFilterGroup.Filters.Add(new EmptyRecordFilter()
            {
                FieldName = "PaidCash"
            });
            orFilterGroup.Filters.Add(new BooleanRecordFilter()
            {
                FieldName = "PaidCash",
                IsTrue = false
            });

            filterGroup.Filters.Add(orFilterGroup);
            filterGroup.Filters.Add(new NumberRecordFilter()
            {
                FieldName = "Charge",
                Value = 0,
                CompareOperator = NumberRecordFilterOperator.Greater
            });

            filterGroup.Filters.Add(new DateTimeRecordFilter()
            {
                FieldName = "ChargeTime",
                Value = fromTime,
                CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals
            });
            filterGroup.Filters.Add(new DateTimeRecordFilter()
            {
                FieldName = "ChargeTime",
                Value = toTime,
                CompareOperator = DateTimeRecordFilterOperator.Less
            });

            return GetContractServiceActions(filterGroup);
        }

        public AddContractServiceActionOutput AddContractServiceAction(AddContractServiceActionInput input)
        {
            var itemToAdd = new GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractServiceAction,
                FieldValues = new Dictionary<string, object>()
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
        #endregion
       
        #region Private Methods
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
        private ContractServiceAction ContractServiceActionMapper(GenericBusinessEntity contractServiceActionEntity)
        {
            return new ContractServiceAction
            {
                ID = (long)contractServiceActionEntity.FieldValues["ID"],
                Contract = (long)contractServiceActionEntity.FieldValues["Contract"],
                ContractService = (long)contractServiceActionEntity.FieldValues["ContractService"],
                ServiceType = (Guid)contractServiceActionEntity.FieldValues["ServiceType"],
                ServiceTypeOption = (Guid?)contractServiceActionEntity.FieldValues["ServiceTypeOption"],
                ActionType = (Guid)contractServiceActionEntity.FieldValues["ActionType"],
                Charge = (decimal)contractServiceActionEntity.FieldValues["Charge"],
                ChargeTime = (DateTime)contractServiceActionEntity.FieldValues["ChargeTime"],
                ContractServiceHistory = (long?)contractServiceActionEntity.FieldValues["ContractServiceHistory"],
                NewSpeedInMbps = (decimal?)contractServiceActionEntity.FieldValues["NewSpeedInMbps"],
                OverriddenCharge = (decimal?)contractServiceActionEntity.FieldValues["OverriddenCharge"],
                PaidCash = (bool)contractServiceActionEntity.FieldValues["PaidCash"],
                OldServiceOption = (Guid?)contractServiceActionEntity.FieldValues["OldServiceOption"],
                NewServiceOption = (Guid?)contractServiceActionEntity.FieldValues["NewServiceOption"],
                OldServiceOptionActivationFee = (Decimal?)contractServiceActionEntity.FieldValues["OldServiceOptionActivationFee"],
                NewServiceOptionActivationFee = (Decimal?)contractServiceActionEntity.FieldValues["NewServiceOptionActivationFee"],
                OldSpeedInMbps = (Decimal?)contractServiceActionEntity.FieldValues["OldSpeedInMbps"],
                CreatedTime = (DateTime)contractServiceActionEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)contractServiceActionEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)contractServiceActionEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)contractServiceActionEntity.FieldValues.GetRecord("LastModifiedBy")
            };
        }

        #endregion
    }
}
