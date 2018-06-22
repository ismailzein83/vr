using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business.RecurringCharges
{
    public class CustomerRecurringChargeTypeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid customerRecurringChargesTypeBEDefinitionId = new Guid("61885a4a-647d-45ea-810a-7028ae9a7f1f");

        #region Public Mehods
        public string GetCustomerRecurringChargeTypeName(long customerRecurringChargeTypeId)
        {
            var customerRecurringChargesTypes = GetCachedCustomerRecurringChargesTypes();
            if (customerRecurringChargesTypes == null)
            {
                return null;
            }
           IEnumerable<CustomerRecurringChargesType> filteredEntities =  customerRecurringChargesTypes.Values.FindAllRecords(x => x.CustomerRecurringChargeTypeId == customerRecurringChargeTypeId);
           return filteredEntities.FirstOrDefault().Name;
        }
        #endregion

        #region Private Methods

        private Dictionary<long, CustomerRecurringChargesType> GetCachedCustomerRecurringChargesTypes()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedCustomerRecurringChargesTypes", customerRecurringChargesTypeBEDefinitionId, () =>
            {
                Dictionary<long, CustomerRecurringChargesType> customerRecurringChargesTypesDic = new Dictionary<long, CustomerRecurringChargesType>();
                var customerRecurringChargesTypesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(customerRecurringChargesTypeBEDefinitionId);
                if (customerRecurringChargesTypesBEDefinitions != null)
                {
                    foreach (var customerRecurringChargesBEDefinition in customerRecurringChargesTypesBEDefinitions)
                    {
                        var fieldValues = customerRecurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var customerRecurringChargeType = new CustomerRecurringChargesType
                            {
                                CustomerRecurringChargeTypeId = Convert.ToInt64(fieldValues.GetRecord("ID")),
                                Name = Convert.ToString(fieldValues.GetRecord("Name"))
                            };
                            customerRecurringChargesTypesDic.Add(customerRecurringChargeType.CustomerRecurringChargeTypeId, customerRecurringChargeType);
                        }
                    }
                }
                return customerRecurringChargesTypesDic;
            });
        }
     
        #endregion
    }
}
