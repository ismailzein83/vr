using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldBusinessEntityType : DataRecordFieldType
    {
        public int BusinessEntityDefinitionId { get; set; }
        public bool IsNullable { get; set; }

        #region Public Methods

        public override Type GetRuntimeType()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException(string.Format("beDefinition '{0}'", this.BusinessEntityDefinitionId));
            if (beDefinition.Settings == null)
                throw new NullReferenceException("beDefinition.Settings");
            if (beDefinition.Settings.IdType == null)
                throw new NullReferenceException("beDefinition.Settings.IdType");
            Type type = Type.GetType(beDefinition.Settings.IdType);
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            IEnumerable<object> selectedEntityIds = ConvertFieldValueToList<object>(value);
            var entityIds = new List<object>();

            if (selectedEntityIds != null)
            {
                if (selectedEntityIds.Count() == 0)
                    return null;
                else
                    entityIds.AddRange(selectedEntityIds);
            }
            else
                entityIds.Add(value);

            BusinessEntityDefinition beDefinition;
            var obj = GetBusinessEntityManager(out beDefinition);
            return GetBusinessEntityManager(out beDefinition).GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityIds = entityIds });
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var fieldValueObjList = fieldValue as List<object>;
            fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

            var beFilter = filterValue as BusinessEntityFieldTypeFilter;

            BusinessEntityDefinition beDefinition;
            IBusinessEntityManager beManager = GetBusinessEntityManager(out beDefinition); // This statement is not necassary
            return beManager.IsMatched(new BusinessEntityMatchContext() { FieldValueIds = fieldValueObjList, FilterIds = beFilter.BusinessEntityIds });
        }

        #endregion

        #region Private Methods

        IBusinessEntityManager GetBusinessEntityManager(out BusinessEntityDefinition businessEntityDefinition)
        {
            BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            businessEntityDefinition = beDefinition;

            if (beDefinition == null)
                throw new NullReferenceException("beDefinition");

            if (beDefinition.Settings == null)
                throw new NullReferenceException("beDefinition.Settings");

            if (beDefinition.Settings.ManagerFQTN == null)
                throw new NullReferenceException("beDefinition.Settings.ManagerFQTN");

            Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);

            if (beManagerType == null)
                throw new NullReferenceException("beManagerType");

            var beManagerInstance = Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("'{0}' does not implement IBusinessEntityManager", beManagerType.Name));

            return beManagerInstance;
        }

        #endregion
    }
}
