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
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            BusinessEntityDefinition beDefinition = GetBusinessEntityDefinition();
           
            if (beDefinition.Settings.IdType == null)
                throw new NullReferenceException("beDefinition.Settings.IdType");
            return Type.GetType(beDefinition.Settings.IdType);
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            IEnumerable<object> selectedEntityIds = FieldTypeHelper.ConvertFieldValueToList<object>(value);
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

            BusinessEntityDefinition beDefinition = GetBusinessEntityDefinition();
            var beManager = GetBusinessEntityManager();
            return beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityIds = entityIds });
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var fieldValueObjList = fieldValue as List<object>;
            fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

            var beFilter = filterValue as BusinessEntityFieldTypeFilter;

            IBusinessEntityManager beManager = GetBusinessEntityManager(); // This statement is not necassary
            return beManager.IsMatched(new BusinessEntityMatchContext() { FieldValueIds = fieldValueObjList, FilterIds = beFilter.BusinessEntityIds });
        }

        #endregion

        #region Private Methods
        BusinessEntityDefinition GetBusinessEntityDefinition()
        {
            var beDefinitionManager = new BusinessEntityDefinitionManager();
            var businessEntityDefinition = beDefinitionManager.GetBusinessEntityDefinition(this.BusinessEntityDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException(String.Format("businessEntityDefinition '{0}'", this.BusinessEntityDefinitionId));
            if (businessEntityDefinition.Settings == null)
                throw new NullReferenceException(String.Format("businessEntityDefinition.Settings. BusinessEntityDefinitionId '{0}'", this.BusinessEntityDefinitionId));
            return businessEntityDefinition;
        }

        IBusinessEntityManager GetBusinessEntityManager()
        {
            var beDefinitionManager = new BusinessEntityDefinitionManager();
            var beManagerInstance = beDefinitionManager.GetBusinessEntityManager(this.BusinessEntityDefinitionId);
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("beManagerInstance. BusinessEntityDefinitionId '{0}'", this.BusinessEntityDefinitionId));

            return beManagerInstance;
        }

        #endregion

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute()
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal" };
        }
    }
}
