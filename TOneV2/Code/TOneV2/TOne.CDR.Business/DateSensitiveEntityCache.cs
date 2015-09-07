using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.CDR.Business;

namespace TOne.CDR.Business
{
    public class DateSensitiveEntityCache<T> where T : IZoneSupplied
    {
        protected static Zone _AnyZone;
        protected static Zone AnyZone
        {
            get
            {
                lock (typeof(BasePricing))
                {
                    if (_AnyZone == null)
                    {
                        _AnyZone = new Zone();
                        _AnyZone.Name = "**** Any Zone ****";
                        _AnyZone.SupplierID = null;
                    }
                }
                return _AnyZone;
            }
        }

        Dictionary<int, List<T>> _supplierEntities = new Dictionary<int, List<T>>();
        Dictionary<int, Dictionary<String, List<T>>> _ourEntities = new Dictionary<int, Dictionary<String, List<T>>>();

        public DateSensitiveEntityCache(DateTime pricingStart, bool IsRepricing)
        {
            Load(null, 0, pricingStart, IsRepricing);
        }

        public DateSensitiveEntityCache(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
        {
            Load(customerId, zoneId, pricingStart, IsRepricing);
        }

        protected void Load(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
        {
            IList<T> all = TOne.CDR.Business.PricingGeneratorEntities<T>.Load(customerId, zoneId, pricingStart, IsRepricing);
            foreach (T entity in all)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                CarrierAccount supplierAccount = carrierAccountManager.GetCarrierAccount(entity.SupplierId);
                CarrierProfile supplierProfile = carrierProfileManager.GetCarrierProfile(supplierAccount.ProfileId);
                CarrierAccount customerAccount = carrierAccountManager.GetCarrierAccount(entity.CustomerId);
                CarrierProfile customerProfile = carrierProfileManager.GetCarrierProfile(customerAccount.ProfileId);

                if (customerAccount==null) continue;
                if (supplierAccount==null) continue;

                int entityZoneID = (entity.ZoneId == null) ? AnyZone.ZoneId : entity.ZoneId;

                // Our Entities
                if (supplierAccount.CarrierAccountId.Equals("SYS"))
                {
                    Dictionary<String, List<T>> zoneEntities = null;
                    if (!_ourEntities.TryGetValue(entityZoneID, out zoneEntities))
                    {
                        zoneEntities = new Dictionary<String, List<T>>();
                        _ourEntities[entityZoneID] = zoneEntities;
                    }
                    List<T> zoneCustomerEntities = null;
                    if (!zoneEntities.TryGetValue(customerAccount.CarrierAccountId, out zoneCustomerEntities))
                    {
                        zoneCustomerEntities = new List<T>();
                        zoneEntities[customerAccount.CarrierAccountId] = zoneCustomerEntities;
                    }
                    zoneEntities[customerAccount.CarrierAccountId].Add(entity);
                }
                // Supplier Entities
                else
                {
                    List<T> zoneEntities = null;
                    if (!_supplierEntities.TryGetValue(entityZoneID, out zoneEntities))
                    {
                        zoneEntities = new List<T>();
                        _supplierEntities[entityZoneID] = zoneEntities;
                    }
                    zoneEntities.Add(entity);
                }
            }
        }

        public bool GetIsEffective(DateTime? BeginEffectiveDate, DateTime? EndEffectiveDate, DateTime when)
        {
            bool isEffective = BeginEffectiveDate.HasValue ? BeginEffectiveDate.Value <= when : true;
            if (isEffective)
                isEffective = EndEffectiveDate.HasValue ? EndEffectiveDate.Value >= when : true;
            return isEffective;
        }

        public List<T> GetEffectiveEntities(String customerID, int zoneID, DateTime whenEffective)
        {
            List<T> effective = new List<T>();
            // Our Entities?
            if (!customerID.Equals("SYS"))
            {
                if (!_ourEntities.ContainsKey(zoneID)) zoneID = AnyZone.ZoneId;
                if (_ourEntities.ContainsKey(zoneID))
                {
                    if (_ourEntities[zoneID].ContainsKey(customerID))
                    {
                        List<T> entities = _ourEntities[zoneID][customerID];
                        foreach (T entity in entities)
                            if (GetIsEffective(entity.BeginEffectiveDate, entity.EndEffectiveDate, whenEffective))
                                effective.Add(entity);
                    }
                }
            }
            // Supplier Entities
            else
            {
                if (!_supplierEntities.ContainsKey(zoneID)) zoneID = AnyZone.ZoneId;
                if (_supplierEntities.ContainsKey(zoneID))
                {
                    List<T> entities = _supplierEntities[zoneID];
                    foreach (T entity in entities)
                        if (GetIsEffective(entity.BeginEffectiveDate, entity.EndEffectiveDate, whenEffective))
                            effective.Add(entity);
                }
            }
            return effective;
        }
    }
}
