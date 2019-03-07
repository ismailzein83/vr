using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class FinancialRecurringChargeTypeManager    
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        public static Guid recurringChargesTypeBEDefinitionId = new Guid("76688fc9-322b-45aa-8d5d-53cc87ade48e");

        #region Public Mehods
        public string GetRecurringChargeTypeName(long recurringChargeTypeId)
        {
            var recurringChargesTypes = GetCachedRecurringChargesTypes();
            if (recurringChargesTypes == null || recurringChargesTypes.Count==0)
            {
                return null;
            }
            IEnumerable<FinancialRecurringChargeType> filteredEntities = recurringChargesTypes.Values.FindAllRecords(x => x.FinancialRecurringChargeTypeId == recurringChargeTypeId);
            return filteredEntities.FirstOrDefault().Name;
        }
        #endregion

        #region Private Methods

        private Dictionary<long, FinancialRecurringChargeType> GetCachedRecurringChargesTypes()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedRecurringChargesTypes", recurringChargesTypeBEDefinitionId,() =>
            {
                Dictionary<long, FinancialRecurringChargeType> recurringChargesTypesDic = new Dictionary<long, FinancialRecurringChargeType>();
                var recurringChargesTypesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(recurringChargesTypeBEDefinitionId);
                if (recurringChargesTypesBEDefinitions != null)
                {
                    foreach (var recurringChargesBEDefinition in recurringChargesTypesBEDefinitions)
                    {
                        var fieldValues = recurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var financialRecurringChargeType = new FinancialRecurringChargeType
                            {
                                FinancialRecurringChargeTypeId = Convert.ToInt64(fieldValues.GetRecord("ID")),
                                Name = Convert.ToString(fieldValues.GetRecord("Name"))
                            };
                            recurringChargesTypesDic.Add(financialRecurringChargeType.FinancialRecurringChargeTypeId, financialRecurringChargeType);
                        }
                    }
                }
                return recurringChargesTypesDic;
            });
        }

        #endregion
    }
}
