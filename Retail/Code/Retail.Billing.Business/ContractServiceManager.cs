using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Billing.Business
{
    public class ContractServiceManager
    {
        #region Variables

        static Guid s_BusinessEntityDefinitionId_ContractService = new Guid("16289a1d-7e2a-4e95-b0c1-fe5775a8efed");
        static Guid s_BusinessEntityDefinitionId_ContractServiceHistory = new Guid("975c6146-25b5-498d-adea-7eae953931d3");
        public static Guid s_StatusId_New = new Guid("5a7c5e9c-0e37-4f73-9c9e-8f6d8f0f7759");
        public static Guid s_StatusId_Active = new Guid("4bbf8829-65b0-4945-997c-7bcb2b49355d");
        public static Guid s_StatusId_Suspended = new Guid("e7067b0f-fa5d-4d9b-904a-973aa6306d8c");
        public static Guid s_StatusId_Inactive = new Guid("f0acd3fd-c310-4a90-8015-832126cf786d");

        static Vanrise.GenericData.Business.GenericBusinessEntityManager s_genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();

        #endregion

        #region Public Methods

        public AddContractServiceOutput AddContractService(AddContractServiceInput input)
        {
            DateTime bet = input.BET != default(DateTime) ? input.BET : DateTime.Now;

            var entityToAddFieldValues = new System.Collections.Generic.Dictionary<string, object>();
            var historyFieldValues = new System.Collections.Generic.Dictionary<string, object>();

            FillFieldValuesForAdd(input, bet, entityToAddFieldValues, historyFieldValues);

            long contractServiceId = InsertContractService(entityToAddFieldValues);
                        
            InsertContractServiceHistory(contractServiceId, historyFieldValues);

            return new AddContractServiceOutput
            {
                ContractServiceId = contractServiceId
            };
        }

        public UpdateContractServiceOutput UpdateContractService(UpdateContractServiceInput input)
        {
            var existingContractService = s_genericBEManager.GetGenericBusinessEntity(input.ContractServiceId, s_BusinessEntityDefinitionId_ContractService);

            existingContractService.ThrowIfNull("contractService", input.ContractServiceId);

            DateTime bet = input.BET != default(DateTime) ? input.BET : DateTime.Now;

            var entityToUpdateFieldValues = new System.Collections.Generic.Dictionary<string, object>();
            var historyFieldValues = new System.Collections.Generic.Dictionary<string, object>();

            FillFieldValuesForUpdate(input, existingContractService, bet, entityToUpdateFieldValues, historyFieldValues);

            List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate> contractServiceHistoryEntitiesToSetEET = GetContractServiceHistoryToSetEET(input.ContractServiceId, bet);

            var contractServiceToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractService,
                GenericBusinessEntityId = input.ContractServiceId,
                FieldValues = entityToUpdateFieldValues
            };

            s_genericBEManager.UpdateGenericBusinessEntity(contractServiceToUpdate);

            if (contractServiceHistoryEntitiesToSetEET != null)
            {
                foreach (var contractHistoryEntityToSetEET in contractServiceHistoryEntitiesToSetEET)
                {
                    s_genericBEManager.UpdateGenericBusinessEntity(contractHistoryEntityToSetEET);
                }
            }

            InsertContractServiceHistory(input.ContractServiceId, historyFieldValues);

            return new UpdateContractServiceOutput();
        }

        #endregion

        #region Private Methods

        private long InsertContractService(Dictionary<string, object> entityToAddFieldValues)
        {
            var contractServiceToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractService,
                FieldValues = entityToAddFieldValues
            };

            var insertContractServiceOutput = s_genericBEManager.AddGenericBusinessEntity(contractServiceToAdd);

            if (insertContractServiceOutput.Result != Vanrise.Entities.InsertOperationResult.Succeeded)
                throw new Exception($"Add Contract Service Failed while inserting the record into the ContractService Table. Result is: '{insertContractServiceOutput.Result.ToString()}'. Error Message: '{insertContractServiceOutput.Message}'");

            return (long)insertContractServiceOutput.InsertedObject.FieldValues["ID"].Value;
        }

        private void InsertContractServiceHistory(long contractServiceId, Dictionary<string, object> historyFieldValues)
        {
            var contractServiceHistoryToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractServiceHistory,
                FieldValues = historyFieldValues
            };

            historyFieldValues.Add("ContractService", contractServiceId);

            s_genericBEManager.AddGenericBusinessEntity(contractServiceHistoryToAdd);
        }

        private void FillFieldValuesForAdd(AddContractServiceInput input, DateTime bet,
            Dictionary<string, object> entityToAddFieldValues, Dictionary<string, object> historyFieldValues)
        {
            entityToAddFieldValues.Add("Contract", input.ContractId);
            entityToAddFieldValues.Add("ServiceType", input.ServiceTypeId);

            if (input.ServiceTypeOptionId.HasValue)
            {
                entityToAddFieldValues.Add("ServiceTypeOption", input.ServiceTypeOptionId);
                historyFieldValues.Add("ServiceTypeOption", input.ServiceTypeOptionId);
            }

            if (input.BillingAccountId.HasValue)
            {
                entityToAddFieldValues.Add("BillingAccount", input.BillingAccountId.Value);
                historyFieldValues.Add("BillingAccount", input.BillingAccountId.Value);
            }

            entityToAddFieldValues.Add("Status", s_StatusId_New);
            historyFieldValues.Add("Status", s_StatusId_New);

            historyFieldValues.Add("BET", bet);

            FillExtraFieldValuesForAdd(
                new ContractServiceManagerFillExtraFieldValuesForAddContext(input, bet, entityToAddFieldValues, historyFieldValues)
                );
        }

        private void FillFieldValuesForUpdate(UpdateContractServiceInput input,
            Vanrise.GenericData.Entities.GenericBusinessEntity existingContractService, DateTime bet,
            Dictionary<string, object> entityToUpdateFieldValues, Dictionary<string, object> historyFieldValues)
        {
            Guid? serviceTypeOptionId;             
            if (input.ServiceTypeOptionId.HasValue)
            {
                serviceTypeOptionId = input.ServiceTypeOptionId.Value;
                entityToUpdateFieldValues.Add("ServiceTypeOption", serviceTypeOptionId);
            }
            else
            {
                serviceTypeOptionId = (Guid?)existingContractService.FieldValues["ServiceTypeOption"];
            }

            Guid statusId;
            Guid? statusReasonId;
            if (input.StatusId.HasValue)
            {
                statusId = input.StatusId.Value;
                statusReasonId = input.StatusReasonId;
                entityToUpdateFieldValues.Add("Status", statusId);
                entityToUpdateFieldValues.Add("StatusReason", statusReasonId);

                if (statusId == s_StatusId_Active && existingContractService.FieldValues["ActivationTime"] == null)
                    entityToUpdateFieldValues.Add("ActivationTime", bet);

                if (statusId == s_StatusId_Inactive && existingContractService.FieldValues["DeactivationTime"] == null)
                    entityToUpdateFieldValues.Add("DeactivationTime", bet);
            }
            else
            {
                statusId = (Guid)existingContractService.FieldValues["Status"];
                statusReasonId = (Guid?)existingContractService.FieldValues["StatusReason"];
            }

            Guid? chargeableRecurringTypeId;
            if (statusId == s_StatusId_Active)
                chargeableRecurringTypeId = new Guid("87fd7d04-84fc-4250-a4c3-2c7d1f52f212");
            else if (statusId == s_StatusId_Suspended)
                chargeableRecurringTypeId = new Guid("b5ff8ef1-52f6-497a-889f-ccb2dce73f27");
            else
                chargeableRecurringTypeId = null;

            entityToUpdateFieldValues.Add("ChargeableRecurringType", chargeableRecurringTypeId);


            historyFieldValues.Add("ServiceTypeOption", serviceTypeOptionId);
            historyFieldValues.Add("ChargeableRecurringType", chargeableRecurringTypeId);
            historyFieldValues.Add("Status", statusId);
            historyFieldValues.Add("StatusReason", statusReasonId);
            historyFieldValues.Add("BET", bet);

            FillExtraFieldValuesForUpdate(
                new ContractServiceManagerFillExtraFieldValuesForUpdateContext(input, existingContractService, bet, entityToUpdateFieldValues, historyFieldValues)
                );
        }

        private List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate> GetContractServiceHistoryToSetEET(long contractServiceId, DateTime effectiveTime)
        {
            var filterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup { Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>() };

            filterGroup.Filters.Add(new Vanrise.GenericData.Entities.ObjectListRecordFilter
            {
                FieldName = "ContractService",
                Values = new System.Collections.Generic.List<object> { contractServiceId }
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
                s_BusinessEntityDefinitionId_ContractServiceHistory,
                new System.Collections.Generic.List<string> { "ID", "ContractService", "BET", "EET" },
                filterGroup);

            var contractServiceHistoriesToUpdate = new List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate>();

            if (contractHistoryEntities != null)
            {
                foreach (var contractServiceHistoryEntity in contractHistoryEntities)
                {
                    var contractServiceHistoryToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
                    {
                        BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_ContractServiceHistory,
                        GenericBusinessEntityId = contractServiceHistoryEntity.FieldValues["ID"],
                        FieldValues = new Dictionary<string, object>()
                    };

                    DateTime? bet = (DateTime?)contractServiceHistoryEntity.FieldValues["BET"];
                    DateTime eetToSet;
                    if (bet.HasValue && bet.Value > effectiveTime)
                        eetToSet = bet.Value;
                    else
                        eetToSet = effectiveTime;

                    contractServiceHistoryToUpdate.FieldValues.Add("EET", eetToSet);

                    contractServiceHistoriesToUpdate.Add(contractServiceHistoryToUpdate);
                }
            }

            return contractServiceHistoriesToUpdate;
        }

        #endregion

        #region Virtual Methods

        public virtual void FillExtraFieldValuesForAdd(IContractServiceManagerFillExtraFieldValuesForAddContext context)
        {
        }

        public virtual void FillExtraFieldValuesForUpdate(IContractServiceManagerFillExtraFieldValuesForUpdateContext context)
        {

        }

        #endregion

        #region Private Classes

        public class ContractServiceManagerFillExtraFieldValuesForAddContext : IContractServiceManagerFillExtraFieldValuesForAddContext
        {
            public ContractServiceManagerFillExtraFieldValuesForAddContext(AddContractServiceInput input, DateTime bet,
            Dictionary<string, object> entityToAddFieldValues, Dictionary<string, object> historyFieldValues)
            {
                this.Input = input;
                this.BET = bet;
                this.EntityToAddFieldValues = entityToAddFieldValues;
                this.HistoryFieldValues = historyFieldValues;
            }

            public AddContractServiceInput Input { get; private set; }

            public DateTime BET { get; private set; }

            public Dictionary<string, object> EntityToAddFieldValues { get; private set; }

            public Dictionary<string, object> HistoryFieldValues { get; private set; }
        }

        public class ContractServiceManagerFillExtraFieldValuesForUpdateContext : IContractServiceManagerFillExtraFieldValuesForUpdateContext
        {
            public ContractServiceManagerFillExtraFieldValuesForUpdateContext(UpdateContractServiceInput input,
            Vanrise.GenericData.Entities.GenericBusinessEntity existingContractService, DateTime bet,
            Dictionary<string, object> entityToUpdateFieldValues, Dictionary<string, object> historyFieldValues)
            {
                this.Input = input;
                this.ExistingContractService = existingContractService;
                this.BET = bet;
                this.EntityToUpdateFieldValues = entityToUpdateFieldValues;
                this.HistoryFieldValues = historyFieldValues;
            }

            public UpdateContractServiceInput Input { get; private set; }

            public Vanrise.GenericData.Entities.GenericBusinessEntity ExistingContractService { get; private set; }

            public DateTime BET { get; private set; }

            public Dictionary<string, object> EntityToUpdateFieldValues { get; private set; }

            public Dictionary<string, object> HistoryFieldValues { get; private set; }
        }

        #endregion
    }
}
