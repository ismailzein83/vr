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
    public struct ServiceTypeIDAndChargeableConditionIDKey
    {
        public Guid ServiceTypeID { get; set; }
        public Guid ChargeableConditionID { get; set; }
        public override int GetHashCode()
        {
            return this.ServiceTypeID.GetHashCode() + this.ChargeableConditionID.GetHashCode();
        }
    }
    public class ContractServiceRecurringChargeTypeManager
    {

        static readonly Guid BeDefinitionId = new Guid("de5f5cce-972f-48c7-ade6-3d81f8d35fa1");
       
        #region Public Methods
        public ContractServiceRecurringChargeType GetContractServiceRecurringChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndChargeableConditionID(Guid contractServiceTypeID, Guid? contractServiceTypeOptionID, Guid chargeableConditionID)
        {
            var serviceTypeIDAndChargeableConditionIDKey = new ServiceTypeIDAndChargeableConditionIDKey
            {
                ServiceTypeID = contractServiceTypeID,
                ChargeableConditionID = chargeableConditionID
            };
            var contractServiceRecurringChargeTypes = GetCachedContractServiceRecurringChargeTypes();
            var recurringChargeTypeByServiceTypeIDAndChargeableConditionID = contractServiceRecurringChargeTypes.GetRecord(serviceTypeIDAndChargeableConditionIDKey);

            if (recurringChargeTypeByServiceTypeIDAndChargeableConditionID == null)
                return null;

            ContractServiceRecurringChargeType recurringChargeTypeByServiceType = null;

            if (contractServiceTypeOptionID.HasValue)
                recurringChargeTypeByServiceType = recurringChargeTypeByServiceTypeIDAndChargeableConditionID.RecurringChargeTypes.GetRecord(contractServiceTypeOptionID.Value);

            if (recurringChargeTypeByServiceType == null)
                return recurringChargeTypeByServiceTypeIDAndChargeableConditionID.DefaultRecurringChargeType;

            return recurringChargeTypeByServiceType;
        }

        #endregion
        
        #region Private Methods
        private Dictionary<ServiceTypeIDAndChargeableConditionIDKey, ContractServiceRecurringChargeTypeByContractServiceTypeOptionID> GetCachedContractServiceRecurringChargeTypes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedContractServiceRecurringChargeTypes", BeDefinitionId, () =>
            {
                var recurringChargeTypesByServiceTypeIDAndChargeableConditionID = new Dictionary<ServiceTypeIDAndChargeableConditionIDKey, ContractServiceRecurringChargeTypeByContractServiceTypeOptionID>();
                var contractServiceRecurringChargeTypes = GetAllContractServiceRecurringChargeTypes();

                if (contractServiceRecurringChargeTypes != null)
                {
                    foreach (var type in contractServiceRecurringChargeTypes)
                    {
                        var serviceTypeIDAndChargeableConditionIDKey = new ServiceTypeIDAndChargeableConditionIDKey
                        {
                            ServiceTypeID = type.ContractServiceType,
                            ChargeableConditionID = type.ChargeableCondition
                        };
                        var recurringChargeTypeByServiceTypeIDAndChargeableConditionID = recurringChargeTypesByServiceTypeIDAndChargeableConditionID.GetOrCreateItem(serviceTypeIDAndChargeableConditionIDKey);

                        if (recurringChargeTypeByServiceTypeIDAndChargeableConditionID.RecurringChargeTypes == null)
                            recurringChargeTypeByServiceTypeIDAndChargeableConditionID.RecurringChargeTypes = new Dictionary<Guid, ContractServiceRecurringChargeType>();

                        if (type.ContractServiceTypeOption.HasValue)
                        {
                            ContractServiceRecurringChargeType chargeType;

                            if (!recurringChargeTypeByServiceTypeIDAndChargeableConditionID.RecurringChargeTypes.TryGetValue(type.ContractServiceTypeOption.Value, out chargeType))
                                recurringChargeTypeByServiceTypeIDAndChargeableConditionID.RecurringChargeTypes.Add(type.ContractServiceTypeOption.Value, type);
                        }
                        else
                        {
                            if (recurringChargeTypeByServiceTypeIDAndChargeableConditionID.DefaultRecurringChargeType == null)
                                recurringChargeTypeByServiceTypeIDAndChargeableConditionID.DefaultRecurringChargeType = type;
                        }
                    }
                }
                return recurringChargeTypesByServiceTypeIDAndChargeableConditionID;
            });
        }
        private IEnumerable<ContractServiceRecurringChargeType> GetAllContractServiceRecurringChargeTypes(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);

            if (entities == null)
                return null;

            return entities.MapRecords(ContractServiceRecurringChargeTypeMapper);
        }
        #endregion

        #region Mappers
        private ContractServiceRecurringChargeType ContractServiceRecurringChargeTypeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            ContractServiceRecurringChargeType recurringChargeType = new ContractServiceRecurringChargeType()
            {
                ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                ContractType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType"),
                ContractServiceType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractServiceType"),
                ChargeableCondition = (Guid)genericBusinessEntity.FieldValues.GetRecord("ChargeableCondition"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };
            var chargeType = genericBusinessEntity.FieldValues.GetRecord("ChargeType");

            if (chargeType != null)
                recurringChargeType.ChargeType = (Guid)chargeType;

            var contractServiceTypeOption = genericBusinessEntity.FieldValues.GetRecord("ContractServiceTypeOption");

            if (contractServiceTypeOption != null)
                recurringChargeType.ContractServiceTypeOption = (Guid)contractServiceTypeOption;

            return recurringChargeType;
        }
        #endregion
    }
    public class ContractServiceRecurringChargeType
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid ContractType { get; set; }
        public Guid ContractServiceType { get; set; }
        public Guid? ContractServiceTypeOption { get; set; }
        public Guid ChargeableCondition { get; set; }
        public Guid? ChargeType { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ContractServiceRecurringChargeTypeByContractServiceTypeOptionID
    {
        public Dictionary<Guid, ContractServiceRecurringChargeType> RecurringChargeTypes { get; set; }
        public ContractServiceRecurringChargeType DefaultRecurringChargeType { get; set; }
    }
}
