using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

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
        static GenericBusinessEntityDefinitionManager s_genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
        static DataRecordTypeManager s_dataRecordTypeManager = new DataRecordTypeManager();

        #endregion

        #region Public Methods

        public AddContractOutput AddContract(AddContractInput input)
        {
            DateTime bet = input.BET != default(DateTime) ? input.BET : DateTime.Now;

            var entityToAddFieldValues = new System.Collections.Generic.Dictionary<string, object>();
            var historyFieldValues = new System.Collections.Generic.Dictionary<string, object>();

            FillFieldValuesForAdd(input, bet, entityToAddFieldValues, historyFieldValues);

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

            FillFieldValuesForUpdate(input, existingContract, bet, entityToUpdateFieldValues, historyFieldValues);

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

        public UpdateContractInternetPackageOutput UpdateContractInternetPackage(UpdateContractInternetPackageInput input)
        {
            var fieldValues = new Dictionary<string, object>();

            AddSpeedInMbpsFieldToFieldValues(fieldValues, input.SpeedInMbps);
            AddSpeedTypeFieldToFieldValues(fieldValues, input.SpeedType);
            AddPackageLimitInGBFieldToFieldValues(fieldValues, input.PackageLimitInGB);

            UpdateContract(input.ContractId, fieldValues, default(DateTime));

            return new UpdateContractInternetPackageOutput();
        }

        public UpdateContractResourceNameOutput UpdateContractResourceName(UpdateContractResourceNameInput input)
        {
            var fieldValues = new Dictionary<string, object>();

            AddResourceNameFieldToFieldValues(fieldValues, input.ResourceName);

            UpdateContract(input.ContractId, fieldValues, default(DateTime));

            return new UpdateContractResourceNameOutput();
        }

        public UpdateContractStatusOutput UpdateContractStatus(UpdateContractStatusInput input)
        {
            var fieldValues = new Dictionary<string, object>();

            AddStatusFieldToFieldValues(fieldValues, input.StatusId);
            AddStatusReasonFieldToFieldValues(fieldValues, input.StatusReasonId);

            UpdateContract(input.ContractId, fieldValues, default(DateTime));

            return new UpdateContractStatusOutput();
        }


        public Contract GetContract(long contractId)
        {
            var contractEntity = s_genericBEManager.GetGenericBusinessEntity(contractId, s_BusinessEntityDefinitionId_Contract);

            contractEntity.ThrowIfNull("contractEntity", contractId);

            return ContractMapper(contractEntity);
        }

        public List<Contract> GetContractsByMainResource(string resourceName)
        {
            var filterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup { Filters = new System.Collections.Generic.List<Vanrise.GenericData.Entities.RecordFilter>() };

            filterGroup.Filters.Add(new Vanrise.GenericData.Entities.StringRecordFilter
            {
                FieldName = "ResourceName",
                Value = resourceName
            });

            var contractEntities = s_genericBEManager.GetAllGenericBusinessEntities(
                s_BusinessEntityDefinitionId_Contract,
                null,
                filterGroup);

            var contracts = new List<Contract>();

            if (contractEntities != null)
            {
                foreach (var contractEntity in contractEntities)
                {
                    Contract contract = ContractMapper(contractEntity);

                    contracts.Add(contract);
                }
            }

            return contracts;
        }

        #endregion

        #region Private Methods

        private Contract ContractMapper(Vanrise.GenericData.Entities.GenericBusinessEntity contractEntity)
        {
            return new Contract
            {
                ContractId = (long)contractEntity.FieldValues["ID"],
                ContractTypeId = (Guid)contractEntity.FieldValues["ContractType"],
                CustomerId = (long)contractEntity.FieldValues["Customer"],
                RatePlanId = (int)contractEntity.FieldValues["RatePlan"],
                MainResourceName = contractEntity.FieldValues["ResourceName"] as string,
                BillingAccountId = (long)contractEntity.FieldValues["BillingAccount"],
                StatusId = (Guid)contractEntity.FieldValues["Status"],
                StatusReasonId = (Guid?)contractEntity.FieldValues["StatusReason"],
                TechnologyId = (Guid?)contractEntity.FieldValues["Technology"],
                NIMPathId = (long?)contractEntity.FieldValues["NIMPath"],
                TelephonyNodeId = (long?)contractEntity.FieldValues["TelephonyNode"],
                InternetNodeId = (long?)contractEntity.FieldValues["InternetNode"],
                SpecialNumberCategoryId = (Guid?)contractEntity.FieldValues["SpecialNumberCategory"],
                SpeedInMbps = (decimal?)contractEntity.FieldValues["SpeedInMbps"],
                SpeedType = (int?)contractEntity.FieldValues["SpeedType"],
                PackageLimitInGB = (int?)contractEntity.FieldValues["PackageLimitInGB"],
                HasInternet = (bool?)contractEntity.FieldValues["HasInternet"],
                HasTelephony = (bool?)contractEntity.FieldValues["HasTelephony"],
                NbOfLinks = (int?)contractEntity.FieldValues["NbOfLinks"]
            };
        }

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

        private void UpdateContract(long contractId, Dictionary<string, object> fieldValues, DateTime bet)
        {
            if (bet == default(DateTime))
                bet = DateTime.Now;

            var existingContractEntity = s_genericBEManager.GetGenericBusinessEntity(contractId, s_BusinessEntityDefinitionId_Contract);

            existingContractEntity.ThrowIfNull("existingContractEntity", contractId);

            Guid statusId;
            Object statusIdAsObj;
            if (fieldValues.TryGetValue("Status", out statusIdAsObj))
            {
                statusId = (Guid)statusIdAsObj;
                if (statusId == s_StatusId_Active && existingContractEntity.FieldValues["ActivationTime"] == null)
                    fieldValues.Add("ActivationTime", bet);

                if (statusId == s_StatusId_Inactive && existingContractEntity.FieldValues["DeactivationTime"] == null)
                    fieldValues.Add("DeactivationTime", bet);
            }
            else
            {
                statusId = (Guid)existingContractEntity.FieldValues["Status"];
            }
            
            Dictionary<string, object> historyFieldValues = new Dictionary<string, object>();

            var historyFields = GetHistoryFields();
            foreach (var field in historyFields)
            {
                object fieldValue;
                if (fieldValues.TryGetValue(field.Key, out fieldValue)
                    || existingContractEntity.FieldValues.TryGetValue(field.Key, out fieldValue))
                {
                    historyFieldValues.Add(field.Key, fieldValue);
                }
            }

            historyFieldValues.Add("BET", bet);

            List<Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate> contractServiceHistoryEntitiesToSetEET = GetContractHistoryToSetEET(contractId, bet);

            var contractToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate
            {
                BusinessEntityDefinitionId = s_BusinessEntityDefinitionId_Contract,
                GenericBusinessEntityId = contractId,
                FieldValues = fieldValues
            };

            s_genericBEManager.UpdateGenericBusinessEntity(contractToUpdate);

            if (contractServiceHistoryEntitiesToSetEET != null)
            {
                foreach (var contractHistoryEntityToSetEET in contractServiceHistoryEntitiesToSetEET)
                {
                    s_genericBEManager.UpdateGenericBusinessEntity(contractHistoryEntityToSetEET);
                }
            }

            InsertContractHistory(contractId, historyFieldValues);
        }

        private void FillFieldValuesForAdd(AddContractInput input, DateTime bet,
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

            if (input.TechnologyId.HasValue)
            {
                entityToAddFieldValues.Add("Technology", input.TechnologyId.Value);
                historyFieldValues.Add("Technology", input.TechnologyId.Value);
            }

            if (input.SpecialNumberCategoryId.HasValue)
            {
                entityToAddFieldValues.Add("SpecialNumberCategory", input.SpecialNumberCategoryId.Value);
                historyFieldValues.Add("SpecialNumberCategory", input.SpecialNumberCategoryId.Value);
            }

            if (input.HasTelephony.HasValue)
            {
                entityToAddFieldValues.Add("HasTelephony", input.HasTelephony.Value);
                historyFieldValues.Add("HasTelephony", input.HasTelephony.Value);
            }

            if (input.NIMPathId.HasValue)
            {
                entityToAddFieldValues.Add("NIMPath", input.NIMPathId.Value);
                historyFieldValues.Add("NIMPath", input.NIMPathId.Value);
            }

            if (input.TelephonyNodeId.HasValue)
            {
                entityToAddFieldValues.Add("TelephonyNode", input.TelephonyNodeId.Value);
                historyFieldValues.Add("TelephonyNode", input.TelephonyNodeId.Value);
            }

            if (input.InternetNodeId.HasValue)
            {
                entityToAddFieldValues.Add("InternetNode", input.InternetNodeId.Value);
                historyFieldValues.Add("InternetNode", input.InternetNodeId.Value);
            }

            if (input.HasInternet.HasValue)
            {
                entityToAddFieldValues.Add("HasInternet", input.HasInternet.Value);
                historyFieldValues.Add("HasInternet", input.HasInternet.Value);
            }

            if (input.SpeedInMbps.HasValue)
            {
                entityToAddFieldValues.Add("SpeedInMbps", input.SpeedInMbps.Value);
                historyFieldValues.Add("SpeedInMbps", input.SpeedInMbps.Value);
            }

            if (input.SpeedType.HasValue)
            {
                entityToAddFieldValues.Add("SpeedType", input.SpeedType.Value);
                historyFieldValues.Add("SpeedType", input.SpeedType.Value);
            }

            if (input.PackageLimitInGB.HasValue)
            {
                entityToAddFieldValues.Add("PackageLimitInGB", input.PackageLimitInGB.Value);
                historyFieldValues.Add("PackageLimitInGB", input.PackageLimitInGB.Value);
            }

            if (input.NbOfLinks.HasValue)
            {
                entityToAddFieldValues.Add("NbOfLinks", input.NbOfLinks.Value);
                historyFieldValues.Add("NbOfLinks", input.NbOfLinks.Value);
            }

            FillExtraFieldValuesForAdd(
                new ContractManagerFillExtraFieldValuesForAddContext(input, bet, entityToAddFieldValues, historyFieldValues)
                );
        }

        private void FillFieldValuesForUpdate(UpdateContractInput input,
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

            FillExtraFieldValuesForUpdate(
                new ContractManagerFillExtraFieldValuesForUpdateContext(input, existingContract, bet, entityToUpdateFieldValues, historyFieldValues)
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

        Dictionary<string, DataRecordField> GetHistoryFields()
        {
            var historyDataRecordTypeId = s_genericBEDefinitionManager.GetGenericBEDataRecordTypeId(s_BusinessEntityDefinitionId_ContractHistory);
            var historyFields = s_dataRecordTypeManager.GetDataRecordTypeFields(historyDataRecordTypeId);

            historyFields.ThrowIfNull("historyFields", s_BusinessEntityDefinitionId_ContractHistory);

            return historyFields;
        }

        private void AddBillingAccountFieldToFieldValues(Dictionary<string, object> fieldValues, long? billingAccountId)
        {
            fieldValues.Add("BillingAccount", billingAccountId);
        }

        private void AddStatusFieldToFieldValues(Dictionary<string, object> fieldValues, Guid statusId)
        {
            fieldValues.Add("Status", statusId);
        }

        private void AddStatusReasonFieldToFieldValues(Dictionary<string, object> fieldValues, Guid? statusReasonId)
        {
            fieldValues.Add("StatusReason", statusReasonId);
        }

        private void AddResourceNameFieldToFieldValues(Dictionary<string, object> fieldValues, string resourceName)
        {
            fieldValues.Add("ResourceName", resourceName);
        }

        private void AddTechnologyFieldToFieldValues(Dictionary<string, object> fieldValues, Guid? technologyId)
        {
            fieldValues.Add("Technology", technologyId);
        }

        private void AddSpecialNumberCategoryFieldToFieldValues(Dictionary<string, object> fieldValues, Guid? specialNumberCategoryId)
        {
            fieldValues.Add("SpecialNumberCategory", specialNumberCategoryId);
        }

        private void AddSpeedInMbpsFieldToFieldValues(Dictionary<string, object> fieldValues, decimal? speedInMbps)
        {
            fieldValues.Add("SpeedInMbps", speedInMbps);
        }

        private void AddSpeedTypeFieldToFieldValues(Dictionary<string, object> fieldValues, int? speedType)
        {
            fieldValues.Add("SpeedType", speedType);
        }

        private void AddPackageLimitInGBFieldToFieldValues(Dictionary<string, object> fieldValues, int? packageLimitInGB)
        {
            fieldValues.Add("PackageLimitInGB", packageLimitInGB);
        }

        #endregion

        #region Virtual Methods

        public virtual void FillExtraFieldValuesForAdd(IContractManagerFillExtraFieldValuesForAddContext context)
        {
        }

        public virtual void FillExtraFieldValuesForUpdate(IContractManagerFillExtraFieldValuesForUpdateContext context)
        {

        }

        #endregion

        #region Private Classes

        public class ContractManagerFillExtraFieldValuesForAddContext : IContractManagerFillExtraFieldValuesForAddContext
        {
            public ContractManagerFillExtraFieldValuesForAddContext(AddContractInput input, DateTime bet,
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

        public class ContractManagerFillExtraFieldValuesForUpdateContext : IContractManagerFillExtraFieldValuesForUpdateContext
        {
            public ContractManagerFillExtraFieldValuesForUpdateContext(UpdateContractInput input,
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
