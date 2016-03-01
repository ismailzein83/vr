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
        #region Public Methods

        public int BusinessEntityDefinitionId { get; set; }

        public override Type GetRuntimeType()
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();
            BusinessEntityDefinition beDefinition = beDefinitionManager.GetBusinessEntityDefinition(BusinessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException(string.Format("beDefinition '{0}'", this.BusinessEntityDefinitionId));
            return Type.GetType(beDefinition.Settings.IdType);
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            IEnumerable<object> selectedEntityIds = ConvertValueToSelectedEntityIds(value);
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
            
            return GetBusinessEntityManager().GetEntityDescription(new BusinessEntityDescriptionContext() { EntityIds = entityIds });
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var fieldValueObjList = fieldValue as List<object>;
            fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

            var beFilter = filterValue as BusinessEntityFieldTypeFilter;

            IBusinessEntityManager beManager = GetBusinessEntityManager();
            return beManager.IsMatched(new BusinessEntityMatchContext() { FieldValueIds = fieldValueObjList, FilterIds = beFilter.BusinessEntityIds });
        }

        #endregion

        #region Private Methods

        IEnumerable<object> ConvertValueToSelectedEntityIds(object value)
        {
            var beValues = value as BusinessEntityValues;
            if (beValues != null)
            {
                var values = beValues.GetValues();
                return (values != null) ? values : new List<object>();
            }
            
            var staticValues = value as StaticValues;
            if (staticValues != null)
                return staticValues.Values;

            var objList = value as List<object>;
            if (objList != null)
                return objList;

            return null;
        }

        IBusinessEntityManager GetBusinessEntityManager()
        {
            BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(BusinessEntityDefinitionId);

            if (beDefinition == null)
                throw new NullReferenceException("beDefinition");

            if (beDefinition.Settings == null)
                throw new NullReferenceException("beDefinition.Settings");

            if (beDefinition.Settings.ManagerFQTN == null)
                throw new NullReferenceException("beDefinition.Settings.ManagerFQTN");

            Type beManagerType = Type.GetType(beDefinition.Settings.ManagerFQTN);

            if (beManagerType == null)
                throw new NullReferenceException("beManagerType");

            return Activator.CreateInstance(beManagerType) as IBusinessEntityManager;
        }

        #endregion
    }
}
