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
    public struct ServiceTypeIDAndActionTypeIDKey
    {
        public Guid ServiceTypeID { get; set; }
        public Guid ActionTypeID { get; set; }
        public override int GetHashCode()
        {
            return this.ServiceTypeID.GetHashCode() + this.ActionTypeID.GetHashCode();
        }
    }
    public class ContractServiceActionChargeTypeManager
    {

        static readonly Guid BeDefinitionId = new Guid("3d777f83-c3c4-400e-824b-67c4c1f3f89a");
       
        #region Public Methods
        public ContractServiceActionChargeType GetContractServiceActionChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndActionTypeID(Guid contractServiceTypeID, Guid? contractServiceTypeOptionID, Guid actionTypeID)
        {
            var serviceTypeIDAndActionTypeIDKey = new ServiceTypeIDAndActionTypeIDKey
            {
                ServiceTypeID = contractServiceTypeID,
                ActionTypeID = actionTypeID
            };
            var contractServiceActionChargeTypes = GetCachedContractServiceActionChargeTypes();
            var actionChargeTypeByServiceTypeIDAndActionTypeID = contractServiceActionChargeTypes.GetRecord(serviceTypeIDAndActionTypeIDKey);

            if (actionChargeTypeByServiceTypeIDAndActionTypeID == null)
                return null;

            ContractServiceActionChargeType actionChargeTypeByServiceType = null;

            if (contractServiceTypeOptionID.HasValue)
                actionChargeTypeByServiceType = actionChargeTypeByServiceTypeIDAndActionTypeID.ActionChargeTypes.GetRecord(contractServiceTypeOptionID.Value);

            if (actionChargeTypeByServiceType == null)
                return actionChargeTypeByServiceTypeIDAndActionTypeID.DefaultActionChargeType;

            return actionChargeTypeByServiceType;
        }

        #endregion
        
        #region Private Methods
        private Dictionary<ServiceTypeIDAndActionTypeIDKey, ContractServiceActionChargeTypeByContractServiceTypeOptionID> GetCachedContractServiceActionChargeTypes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedContractServiceActionChargeTypes", BeDefinitionId, () =>
            {
                var actionChargeTypesByServiceTypeIDAndActionTypeID = new Dictionary<ServiceTypeIDAndActionTypeIDKey, ContractServiceActionChargeTypeByContractServiceTypeOptionID>();
                var contractServiceActionChargeTypes = GetAllContractServiceActionChargeTypes();

                if (contractServiceActionChargeTypes != null)
                {
                    foreach (var type in contractServiceActionChargeTypes)
                    {
                        var serviceTypeIDAndActionTypeIDKey = new ServiceTypeIDAndActionTypeIDKey
                        {
                            ServiceTypeID = type.ContractServiceType,
                            ActionTypeID = type.ActionType
                        };
                        var actionChargeTypeByServiceTypeIDAndActionTypeID = actionChargeTypesByServiceTypeIDAndActionTypeID.GetOrCreateItem(serviceTypeIDAndActionTypeIDKey);
                        if (actionChargeTypeByServiceTypeIDAndActionTypeID.ActionChargeTypes == null)
                            actionChargeTypeByServiceTypeIDAndActionTypeID.ActionChargeTypes = new Dictionary<Guid, ContractServiceActionChargeType>();

                        if (type.ContractServiceTypeOption.HasValue)
                        {
                            ContractServiceActionChargeType chargeType;
                            if (!actionChargeTypeByServiceTypeIDAndActionTypeID.ActionChargeTypes.TryGetValue(type.ContractServiceTypeOption.Value, out chargeType))
                                actionChargeTypeByServiceTypeIDAndActionTypeID.ActionChargeTypes.Add(type.ContractServiceTypeOption.Value, type);
                        }
                        else
                        {
                            if (actionChargeTypeByServiceTypeIDAndActionTypeID.DefaultActionChargeType == null)
                                actionChargeTypeByServiceTypeIDAndActionTypeID.DefaultActionChargeType = type;
                        }
                    }
                }
                return actionChargeTypesByServiceTypeIDAndActionTypeID;
            });
        }
        private IEnumerable<ContractServiceActionChargeType> GetAllContractServiceActionChargeTypes(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;

            return entities.MapRecords(ContractServiceActionChargeTypeMapper);
        }
        #endregion

        #region Mappers
        private ContractServiceActionChargeType ContractServiceActionChargeTypeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            ContractServiceActionChargeType actionChargeType = new ContractServiceActionChargeType()
            {
                ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                ContractType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType"),
                ContractServiceType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractServiceType"),
                ActionType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ActionType"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };
            var chargeType = genericBusinessEntity.FieldValues.GetRecord("ChargeType");
            if (chargeType != null)
                actionChargeType.ChargeType = (Guid)chargeType;

            var contractServiceTypeOption = genericBusinessEntity.FieldValues.GetRecord("ContractServiceTypeOption");
            if (contractServiceTypeOption != null)
                actionChargeType.ContractServiceTypeOption = (Guid)contractServiceTypeOption;

            var advancedPaymentChargeType = genericBusinessEntity.FieldValues.GetRecord("AdvancedPaymentChargeType");
            if (advancedPaymentChargeType != null)
                actionChargeType.AdvancedPaymentChargeType = (Guid)advancedPaymentChargeType;

            var depositChargeType = genericBusinessEntity.FieldValues.GetRecord("DepositChargeType");
            if (depositChargeType != null)
                actionChargeType.DepositChargeType = (Guid)depositChargeType;

            return actionChargeType;
        }
        #endregion
    }
    public class ContractServiceActionChargeType
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid ContractType { get; set; }
        public Guid ContractServiceType { get; set; }
        public Guid? ContractServiceTypeOption { get; set; }
        public Guid ActionType { get; set; }
        public Guid? ChargeType { get; set; }
        public Guid? AdvancedPaymentChargeType { get; set; }
        public Guid? DepositChargeType { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ContractServiceActionChargeTypeByContractServiceTypeOptionID
    {
        public Dictionary<Guid, ContractServiceActionChargeType> ActionChargeTypes { get; set; }
        public ContractServiceActionChargeType DefaultActionChargeType { get; set; }
    }
}
