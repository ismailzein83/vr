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
    public class BiilingContractServiceChargeableConditionManager
    {
        static readonly Guid BeDefinitionId = new Guid("2e3d885e-d561-4da8-8c16-013358a034c0");

        #region Public Methods
        public List<BillingContractServiceChargeableCondition> GetCachedContractServiceChargeableCondition()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedContractServiceChargeableCondition", BeDefinitionId, () =>
            {
                var contractServiceChargeableConditions = GetAllContractServiceChargeableConditions();
                var contractServiceChargeableConditionsOrderedByPriority = new List<BillingContractServiceChargeableCondition>();

                if (contractServiceChargeableConditions != null)
                    contractServiceChargeableConditionsOrderedByPriority = contractServiceChargeableConditions.OrderBy(x => x.Priority).ToList();

                return contractServiceChargeableConditionsOrderedByPriority;
            });
        }
        #endregion

        #region Private Methods
        private IEnumerable<BillingContractServiceChargeableCondition> GetAllContractServiceChargeableConditions(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;

            return entities.MapRecords(ContractServiceChargeableConditionMapper);
        }

        private BillingContractServiceChargeableCondition ContractServiceChargeableConditionMapper(GenericBusinessEntity genericBusinessEntity)
        {
            return new BillingContractServiceChargeableCondition()
            {
                ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                Condition = (RecordFilterGroup)genericBusinessEntity.FieldValues.GetRecord("Condition"),
                Priority = (int)genericBusinessEntity.FieldValues.GetRecord("Priority"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };
        }

        #endregion

        #region Classes
        public class BillingContractServiceChargeableCondition
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public RecordFilterGroup Condition { get; set; }
            public int Priority { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public int CreatedBy { get; set; }
            public int LastModifiedBy { get; set; }
        }
        #endregion
    }
}
