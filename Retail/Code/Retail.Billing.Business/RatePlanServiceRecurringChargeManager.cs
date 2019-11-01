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
    public class RatePlanServiceRecurringChargeManager
    {

        static readonly Guid BeDefinitionId = new Guid("af096ddc-96c1-46f9-8dff-c9651602b083");

        public RatePlanServiceRecurringCharge GetRatePlanServiceRecurringCharge(int ratePlanId, Guid recurringChargeTypeID)
        {
            var serviceRecurringCharges = GetCachedRatePlanServiceRecurringChargesByRatePlanIDAndRecurringChargeTypeID();
            var serviceRecurringChargesByRatePlanId = serviceRecurringCharges.GetRecord(ratePlanId);

            if (serviceRecurringChargesByRatePlanId == null)
                return null;

            return serviceRecurringChargesByRatePlanId.GetRecord(recurringChargeTypeID);
        }

        private Dictionary<int, Dictionary<Guid, RatePlanServiceRecurringCharge>> GetCachedRatePlanServiceRecurringChargesByRatePlanIDAndRecurringChargeTypeID()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRatePlanServiceRecurringChargesByRatePlanIDAndRecurringChargeTypeID", BeDefinitionId, () =>
            {
                var ratePlanServiceRecurringCharges = GetAllRatePlanServiceRecurringCharges();

                var serviceRecurringChargesByRatePlanIDAndRecurringChargeTypeID = new Dictionary<int, Dictionary<Guid, RatePlanServiceRecurringCharge>>();

                if (ratePlanServiceRecurringCharges != null)
                {
                    foreach (var charge in ratePlanServiceRecurringCharges)
                    {
                        var ratePlanServiceRecurringChargeByRatePlanID = serviceRecurringChargesByRatePlanIDAndRecurringChargeTypeID.GetOrCreateItem(charge.RatePlan);
                        ratePlanServiceRecurringChargeByRatePlanID.Add(charge.RecurringChargeType, charge);
                    }
                }
                return serviceRecurringChargesByRatePlanIDAndRecurringChargeTypeID;
            });
        }
        private IEnumerable<RatePlanServiceRecurringCharge> GetAllRatePlanServiceRecurringCharges(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);
            if (entities == null)
                return null;

            return entities.MapRecords(RatePlanServiceRecurringChargeTypeMapper);
        }

        private RatePlanServiceRecurringCharge RatePlanServiceRecurringChargeTypeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            RatePlanServiceRecurringCharge recurringChargeType = new RatePlanServiceRecurringCharge()
            {
                ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                RatePlan = (int)genericBusinessEntity.FieldValues.GetRecord("RatePlan"),
                ContractType= (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType"),
                RecurringChargeType = (Guid)genericBusinessEntity.FieldValues.GetRecord("RecurringChargeType"),
                Charge = (RetailBillingCharge)genericBusinessEntity.FieldValues.GetRecord("Charge"),
                RecurringPeriod = (int)genericBusinessEntity.FieldValues.GetRecord("RecurringPeriod"),
                ProrateOnStart = (bool)genericBusinessEntity.FieldValues.GetRecord("ProrateOnStart"),
                ProrateOnEnd = (bool)genericBusinessEntity.FieldValues.GetRecord("ProrateOnEnd"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };

            var currency = genericBusinessEntity.FieldValues.GetRecord("Currency");

            if (currency != null)
                recurringChargeType.Currency = (int)currency;

            return recurringChargeType;
        }
    }
    public class RatePlanServiceRecurringCharge
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RatePlan { get; set; }
        public Guid ContractType { get; set; }
        public Guid RecurringChargeType { get; set; }
        public RetailBillingCharge Charge { get; set; }
        public int? Currency { get; set; }
        public int RecurringPeriod { get; set; }
        public bool ProrateOnStart { get; set; }
        public bool ProrateOnEnd { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
}
