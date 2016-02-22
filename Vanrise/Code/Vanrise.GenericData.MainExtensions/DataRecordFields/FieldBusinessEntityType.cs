using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldBusinessEntityType : DataRecordFieldType
    {
        public int BusinessEntityDefinitionId { get; set; }

        public override Type GetRuntimeType()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException(string.Format("beDefinition '{0}'", this.BusinessEntityDefinitionId));
            return Type.GetType(beDefinition.Settings.IdType);
        }

        public override string GetDescription(Object value)
        {
            var beValues = value as BusinessEntityValues;
            var entityIds = new List<object>();

            if (beValues == null)
                entityIds.Add(value);
            else
            {
                var selectedValues = beValues.GetValues();
                foreach (var beValue in selectedValues)
                    entityIds.Add(beValue);
            }

            IBusinessEntityManager beManager = GetBusinessEntityManager();
            return beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityIds = entityIds });
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var fieldValueObjList = fieldValue as List<object>;
            var beFilter = filterValue as BusinessEntityFieldTypeFilter;

            IBusinessEntityManager beManager = GetBusinessEntityManager();
            return beManager.IsMatched(new BusinessEntityMatchContext() { FieldValueIds = fieldValueObjList, FilterIds = beFilter.BusinessEntityIds });
        }

        IBusinessEntityManager GetBusinessEntityManager()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition != null && beDefinition.Settings != null && beDefinition.Settings.ManagerFQTN != null)
            {
                Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);
                return Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
            }
            return null;
        }
    }
}
