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
    public class RatePlanServiceActionChargeManager
    {
        static readonly Guid BeDefinitionId = new Guid("36a2f676-cc28-4d0e-95d5-d66bc766c169");

        public RatePlanServiceActionCharge GetRatePlanServiceActionCharge(int ratePlanId, Guid actionChargeTypeID)
        {
            var serviceActionCharges = GetCachedRatePlanServiceActionChargesByRatePlanIDAndActionChargeTypeID();
            var serviceActionChargesByRatePlanId = serviceActionCharges.GetRecord(ratePlanId);

            if (serviceActionChargesByRatePlanId == null)
                return null;

            return serviceActionChargesByRatePlanId.GetRecord(actionChargeTypeID);
        }

        private Dictionary<int, Dictionary<Guid, RatePlanServiceActionCharge>> GetCachedRatePlanServiceActionChargesByRatePlanIDAndActionChargeTypeID()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRatePlanServiceActionChargesByRatePlanIDAndActionChargeTypeID", BeDefinitionId, () =>
            {
                var ratePlanServiceActionCharges = GetAllRatePlanServiceActionCharges();
                var serviceActionChargesByRatePlanIDAndActionChargeTypeID = new Dictionary<int, Dictionary<Guid, RatePlanServiceActionCharge>>();

                if (ratePlanServiceActionCharges != null)
                {
                    foreach (var charge in ratePlanServiceActionCharges)
                    {
                        var ratePlanServiceActionChargeByRatePlanID = serviceActionChargesByRatePlanIDAndActionChargeTypeID.GetOrCreateItem(charge.RatePlan);
                        ratePlanServiceActionChargeByRatePlanID.Add(charge.ActionChargeType, charge);
                    }
                }
                return serviceActionChargesByRatePlanIDAndActionChargeTypeID;
            });
        }

        private IEnumerable<RatePlanServiceActionCharge> GetAllRatePlanServiceActionCharges(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;

            return entities.MapRecords(RatePlanServiceActionChargeTypeMapper);
        }

        private RatePlanServiceActionCharge RatePlanServiceActionChargeTypeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            RatePlanServiceActionCharge ActionChargeType = new RatePlanServiceActionCharge()
            {
                ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                RatePlan = (int)genericBusinessEntity.FieldValues.GetRecord("RatePlan"),
                ContractType= (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType"),
                ActionChargeType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ActionChargeType"),
                Charge = (RetailBillingCharge)genericBusinessEntity.FieldValues.GetRecord("Charge"),
                AdvancedPayment = (RetailBillingCharge)genericBusinessEntity.FieldValues.GetRecord("AdvancedPayment"),
                Deposit = (RetailBillingCharge)genericBusinessEntity.FieldValues.GetRecord("Deposit"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };

            var currency = genericBusinessEntity.FieldValues.GetRecord("Currency");
            if (currency != null)
                ActionChargeType.Currency = (int)currency;

            return ActionChargeType;
        }
    }
    public class RatePlanServiceActionCharge
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RatePlan { get; set; }
        public Guid ContractType { get; set; }
        public Guid ActionChargeType { get; set; }
        public RetailBillingCharge Charge { get; set; }
        public RetailBillingCharge AdvancedPayment { get; set; }
        public RetailBillingCharge Deposit { get; set; }
        public int? Currency { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
}
