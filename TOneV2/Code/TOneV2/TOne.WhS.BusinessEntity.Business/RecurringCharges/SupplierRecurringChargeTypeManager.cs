using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRecurringChargeTypeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid supplierRecurringChargesTypeBEDefinitionId = new Guid("7501c2b2-374b-432c-b423-59e772663eb8");

        #region Public Mehods
        public string GetSupplierRecurringChargeTypeName(long supplierRecurringChargeTypeId)
        {
            var supplierRecurringChargesTypes = GetCachedSupplierRecurringChargesTypes();
            if (supplierRecurringChargesTypes == null)
            {
                return null;
            }
            IEnumerable<SupplierRecurringChargesType> filteredEntities = supplierRecurringChargesTypes.Values.FindAllRecords(x => x.SupplierRecurringChargeTypeId == supplierRecurringChargeTypeId);
            return filteredEntities.FirstOrDefault().Name;
        }
        #endregion

        #region Private Methods

        private Dictionary<long, SupplierRecurringChargesType> GetCachedSupplierRecurringChargesTypes()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedSupplierRecurringChargesTypes", supplierRecurringChargesTypeBEDefinitionId, () =>
            {
                Dictionary<long, SupplierRecurringChargesType> supplierRecurringChargesTypesDic = new Dictionary<long, SupplierRecurringChargesType>();
                var supplierRecurringChargesTypesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(supplierRecurringChargesTypeBEDefinitionId);
                if (supplierRecurringChargesTypesBEDefinitions != null)
                {
                    foreach (var supplierRecurringChargesBEDefinition in supplierRecurringChargesTypesBEDefinitions)
                    {
                        var fieldValues = supplierRecurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var supplierRecurringChargeType = new SupplierRecurringChargesType
                            {
                                SupplierRecurringChargeTypeId = Convert.ToInt64(fieldValues.GetRecord("ID")),
                                Name = Convert.ToString(fieldValues.GetRecord("Name"))
                            };
                            supplierRecurringChargesTypesDic.Add(supplierRecurringChargeType.SupplierRecurringChargeTypeId, supplierRecurringChargeType);
                        }
                    }
                }
                return supplierRecurringChargesTypesDic;
            });
        }

        #endregion

    }
}
