﻿using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Billing.Business
{
    public class ContractManager
    {
        #region Variables

        static Guid s_BusinessEntityDefinitionId_Contract = new Guid("7c323462-f1f8-4b57-ad94-d1b5bf3aef08");
        static Guid s_BusinessEntityDefinitionId_ContractHistory = new Guid("4f429531-fb9c-4997-a0d7-96cc579f001e");

        static Guid s_StatusId_New = new Guid("26a5a0df-b458-426d-8d7f-9e7767ffe7a1");
        static Guid s_StatusId_Active = new Guid("9ec67e6c-028c-4361-9e9b-759941899e8e");
        static Guid s_StatusId_Suspended = new Guid("e712f929-80f9-4f46-88e5-eceef3d365ac");
        static Guid s_StatusId_Inactive = new Guid("557a93b0-d305-422f-9267-1562868984fa");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        #endregion

        #region Public Methods

        public AddContractOutput AddContract(AddContractInput input)
        {
            DateTime bet = input.BET != default(DateTime) ? input.BET : DateTime.Now;

            var entityToAddFieldValues = new System.Collections.Generic.Dictionary<string, object>();
            var historyFieldValues = new System.Collections.Generic.Dictionary<string, object>();

            FillFieldValuesFromAddContractInput(input, bet, entityToAddFieldValues, historyFieldValues);

            long contractId = InsertContract(entityToAddFieldValues);
            InsertContractHistory(contractId, historyFieldValues);

            return new AddContractOutput
            {
                ContractId = contractId
            };
        }

        public UpdateContractOutput UpdateContract(UpdateContractInput input)
        {
            var existingContract = s_genericBEManager.GetGenericBusinessEntity(input.ContractId, s_BusinessEntityDefinitionId_Contract);

            existingContract.ThrowIfNull("existingContract", input.ContractId);

            DateTime bet = input.BET != default(DateTime) ? input.BET : DateTime.Now;

            var entityToUpdateFieldValues = new System.Collections.Generic.Dictionary<string, object>();
            var historyFieldValues = new System.Collections.Generic.Dictionary<string, object>();

            FillFieldValuesFromUpdateContractInput(input, existingContract, bet, entityToUpdateFieldValues, historyFieldValues);

            var contractHistoryEntitiesToSetEET = GetContractHistoryToSetEET(input.ContractId, bet);

            var contractToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Contract,
                GenericBusinessEntityId = input.ContractId,
                FieldValues = entityToUpdateFieldValues
            };

            s_genericBEManager.UpdateGenericBusinessEntity(contractToUpdate);

            if (contractHistoryEntitiesToSetEET != null)
            {
                foreach (var contractHistoryEntityToSetEET in contractHistoryEntitiesToSetEET)
                {
                    s_genericBEManager.UpdateGenericBusinessEntity(contractHistoryEntityToSetEET);
                }
            }

            InsertContractHistory(input.ContractId, historyFieldValues);

            return new UpdateContractOutput();
        }

        #endregion
        
        #region Private Methods

        private long InsertContract(Dictionary<string, object> entityToAddFieldValues)
        {
            var contractToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Contract,
                FieldValues = entityToAddFieldValues
            };

            var insertContractOutput = s_genericBEManager.AddGenericBusinessEntity(contractToAdd);

            if (insertContractOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Contract Failed while inserting the record into the Contract Table. Result is: '{insertContractOutput.Result.ToString()}'. Error Message: '{insertContractOutput.Message}'");

            var contractId = (long)insertContractOutput.InsertedObject.FieldValues["ID"].Value;
            return contractId;
        }

        private void InsertContractHistory(long contractId, Dictionary<string, object> historyFieldValues)
        {
            var contractHistoryToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractHistory,
                FieldValues = historyFieldValues
            };

            historyFieldValues.Add("Contract", contractId);

            s_genericBEManager.AddGenericBusinessEntity(contractHistoryToAdd);
        }

        private void FillFieldValuesFromAddContractInput(AddContractInput input, DateTime bet,
            Dictionary<string, object> entityToAddFieldValues, Dictionary<string, object> historyFieldValues)
        {
            entityToAddFieldValues.Add("ContractType", input.ContractTypeId);
            entityToAddFieldValues.Add("Customer", input.CustomerId);
            entityToAddFieldValues.Add("BillingAccount", input.BillingAccountId);
            entityToAddFieldValues.Add("RatePlan", input.RatePlanId);
            entityToAddFieldValues.Add("ResourceName", input.MainResourceName);
            entityToAddFieldValues.Add("Status", s_StatusId_New);

            historyFieldValues.Add("BillingAccount", input.BillingAccountId);
            historyFieldValues.Add("RatePlan", input.RatePlanId);
            historyFieldValues.Add("MainResourceName", input.MainResourceName);
            historyFieldValues.Add("Status", s_StatusId_New);
            historyFieldValues.Add("BET", bet);

            FillExtraFieldValuesFromAddContractInput(
                new ContractManagerFillExtraFieldValuesFromAddContractInputContext(input, bet, entityToAddFieldValues, historyFieldValues)
                );
        }

        private void FillFieldValuesFromUpdateContractInput(UpdateContractInput input,
            Vanrise.GenericData.Entities.GenericBusinessEntity existingContract, DateTime bet,
            Dictionary<string, object> entityToUpdateFieldValues, Dictionary<string, object> historyFieldValues)
        {
            long billingAccountId;
            if (input.BillingAccountId.HasValue)
            {
                billingAccountId = input.BillingAccountId.Value;
                entityToUpdateFieldValues.Add("BillingAccount", billingAccountId);
            }
            else
            {
                billingAccountId = (long)existingContract.FieldValues["BillingAccount"];
            }

            int ratePlanId;
            if (input.RatePlanId.HasValue)
            {
                ratePlanId = input.RatePlanId.Value;
                entityToUpdateFieldValues.Add("RatePlan", ratePlanId);
            }
            else
            {
                ratePlanId = (int)existingContract.FieldValues["RatePlan"];
            }

            string mainResourceName;
            if (input.MainResourceName != null)
            {
                mainResourceName = input.MainResourceName;
                entityToUpdateFieldValues.Add("ResourceName", mainResourceName);
            }
            else
            {
                mainResourceName = existingContract.FieldValues["ResourceName"] as string;
            }

            Guid statusId;
            Guid? statusReasonId;
            if (input.StatusId.HasValue)
            {
                statusId = input.StatusId.Value;
                statusReasonId = input.StatusReasonId;
                entityToUpdateFieldValues.Add("Status", statusId);
                entityToUpdateFieldValues.Add("StatusReason", statusReasonId);

                if (statusId == s_StatusId_Active && existingContract.FieldValues["ActivationTime"] == null)
                    entityToUpdateFieldValues.Add("ActivationTime", bet);

                if (statusId == s_StatusId_Inactive && existingContract.FieldValues["DeactivationTime"] == null)
                    entityToUpdateFieldValues.Add("DeactivationTime", bet);
            }
            else
            {
                statusId = (Guid)existingContract.FieldValues["Status"];
                statusReasonId = (Guid?)existingContract.FieldValues["StatusReason"];
            }

            historyFieldValues.Add("BillingAccount", billingAccountId);
            historyFieldValues.Add("RatePlan", ratePlanId);
            historyFieldValues.Add("MainResourceName", mainResourceName);
            historyFieldValues.Add("Status", statusId);
            historyFieldValues.Add("StatusReason", statusReasonId);
            historyFieldValues.Add("BET", bet);

            FillExtraFieldValuesFromUpdateContractInput(
                new ContractManagerFillExtraFieldValuesFromUpdateContractInputContext(input, existingContract, bet, entityToUpdateFieldValues, historyFieldValues)
                );
        }

        private List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate> GetContractHistoryToSetEET(long contractId, DateTime effectiveTime)
        {
            var filterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup { Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>() };

            filterGroup.Filters.Add(new Vanrise.GenericData.Entities.ObjectListRecordFilter
            {
                FieldName = "Contract",
                Values = new System.Collections.Generic.List<object> { contractId }
            });

            var orFilterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup
            {
                Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>(),
                LogicalOperator = Vanrise.GenericData.Entities.RecordQueryLogicalOperator.Or
            };
            filterGroup.Filters.Add(orFilterGroup);

            orFilterGroup.Filters.Add(new Vanrise.GenericData.Entities.EmptyRecordFilter { FieldName = "EET" });
            orFilterGroup.Filters.Add(new Vanrise.GenericData.Entities.DateTimeRecordFilter
            {
                FieldName = "EET",
                CompareOperator = Vanrise.GenericData.Entities.DateTimeRecordFilterOperator.Greater,
                Value = effectiveTime
            });

            var contractHistoryEntities = s_genericBEManager.GetAllGenericBusinessEntities(
                s_BusinessEntityDefinitionId_ContractHistory,
                new System.Collections.Generic.List<string> { "ID", "Contract", "BET", "EET" },
                filterGroup);

            var contractHistoriesToUpdate = new List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate>();

            if (contractHistoryEntities != null)
            {
                foreach (var contractHistoryEntity in contractHistoryEntities)
                {
                    var contractHistoryToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
                    {
                        BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractHistory,
                        GenericBusinessEntityId = contractHistoryEntity.FieldValues["ID"],
                        FieldValues = new Dictionary<string, object>()
                    };

                    DateTime? bet = (DateTime?)contractHistoryEntity.FieldValues["BET"];
                    DateTime eetToSet;
                    if (bet.HasValue && bet.Value > effectiveTime)
                        eetToSet = bet.Value;
                    else
                        eetToSet = effectiveTime;

                    contractHistoryToUpdate.FieldValues.Add("EET", eetToSet);

                    contractHistoriesToUpdate.Add(contractHistoryToUpdate);
                }
            }

            return contractHistoriesToUpdate;
        }

        #endregion

        #region Virtual Methods

        public virtual void FillExtraFieldValuesFromAddContractInput(IContractManagerFillExtraFieldValuesFromAddContractInputContext context)
        {
        }

        public virtual void FillExtraFieldValuesFromUpdateContractInput(IContractManagerFillExtraFieldValuesFromUpdateContractInputContext context)
        {

        }

        #endregion

        #region Private Classes

        public class ContractManagerFillExtraFieldValuesFromAddContractInputContext : IContractManagerFillExtraFieldValuesFromAddContractInputContext
        {
            public ContractManagerFillExtraFieldValuesFromAddContractInputContext(AddContractInput input, DateTime bet,
            Dictionary<string, object> entityToAddFieldValues, Dictionary<string, object> historyFieldValues)
            {
                this.Input = input;
                this.BET = bet;
                this.EntityToAddFieldValues = entityToAddFieldValues;
                this.HistoryFieldValues = historyFieldValues;
            }

            public AddContractInput Input { get; private set; }

            public DateTime BET { get; private set; }

            public Dictionary<string, object> EntityToAddFieldValues { get; private set; }

            public Dictionary<string, object> HistoryFieldValues { get; private set; }
        }

        public class ContractManagerFillExtraFieldValuesFromUpdateContractInputContext : IContractManagerFillExtraFieldValuesFromUpdateContractInputContext
        {
            public ContractManagerFillExtraFieldValuesFromUpdateContractInputContext(UpdateContractInput input,
            Vanrise.GenericData.Entities.GenericBusinessEntity existingContract, DateTime bet,
            Dictionary<string, object> entityToUpdateFieldValues, Dictionary<string, object> historyFieldValues)
            {
                this.Input = input;
                this.ExistingContract = existingContract;
                this.BET = bet;
                this.EntityToUpdateFieldValues = entityToUpdateFieldValues;
                this.HistoryFieldValues = historyFieldValues;
            }

            public UpdateContractInput Input { get; private set; }

            public Vanrise.GenericData.Entities.GenericBusinessEntity ExistingContract { get; private set; }

            public DateTime BET { get; private set; }

            public Dictionary<string, object> EntityToUpdateFieldValues { get; private set; }

            public Dictionary<string, object> HistoryFieldValues { get; private set; }
        }

        #endregion
    }
}
