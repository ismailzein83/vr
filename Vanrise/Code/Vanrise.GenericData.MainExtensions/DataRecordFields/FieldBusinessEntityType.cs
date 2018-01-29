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
        public override Guid ConfigId { get { return new Guid("2e16c3d4-837b-4433-b80e-7c02f6d71467"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-businessentity-runtimeeditor"; } }

        public Guid BusinessEntityDefinitionId { get; set; }
        
        public bool IsNullable { get; set; }
        
        public BERuntimeSelectorFilter BERuntimeSelectorFilter { get; set; }
        
        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }

        #region Public Methods

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }
        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            var type = GetNonNullableRuntimeType();
            if (type.FullName == "System.Guid")
            {
                var newGuidValue = Guid.Parse(newValue.ToString());
                var oldGuidValue = Guid.Parse(oldValue.ToString());
                return newGuidValue.Equals(oldValue);

            }
            else
            {
                var newTypeValue = Convert.ChangeType(newValue, type);
                var oldTypeValue = Convert.ChangeType(oldValue, type);
                return newTypeValue.Equals(oldTypeValue);
            }
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
            {
                return new BusinessEntityDefinitionManager().GetBusinessEntityNullDisplayText(this.BusinessEntityDefinitionId);
            }

            var beValues = value as BusinessEntityValues;
            if (beValues != null)
            {
                return beValues.GetDescription();
            }
            else
            {
                BusinessEntityDefinition beDefinition = GetBusinessEntityDefinition();
                var beManager = GetBusinessEntityManager();
                var entityDescriptions = new List<string>();

                IEnumerable<object> valueAsList = FieldTypeHelper.ConvertFieldValueToList<object>(value);
                int fullCount = 0;
                if (valueAsList != null)
                {
                    if (valueAsList.Count() == 0)
                        return null;
                    else
                    {
                        fullCount = valueAsList.Count();
                        int takenCount = 0;
                        foreach (var entityId in valueAsList)
                        {
                            if(takenCount > 10)
                            {
                                continue;
                            }
                            entityDescriptions.Add(beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityId = entityId }));
                            takenCount++;
                        }
                    }
                }
                else
                {
                    fullCount = 1;
                    entityDescriptions.Add(beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityId = value }));
                }
                return String.Concat(String.Join(",", entityDescriptions), entityDescriptions.Count != fullCount ? String.Format(" ...... ({0} selected)", fullCount) : "");
            }
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue == null || filterValue == null)
                return true;
            var beFilter = filterValue as BusinessEntityFieldTypeFilter;
            if (beFilter == null)
                throw new NullReferenceException("beFilter");
            if (beFilter.BusinessEntityIds.Count == 0)
                return false;
            Type filterValueType = beFilter.BusinessEntityIds[0].GetType();
            var fieldValueAsList = fieldValue as List<object>;
            if (fieldValueAsList != null)
            {
                foreach (var value in fieldValueAsList)
                {
                    if (beFilter.BusinessEntityIds.Contains(Convert.ChangeType(value, filterValueType)))
                        return true;
                }
                return false;
            }
            else
            {
                return beFilter.BusinessEntityIds.Contains(Convert.ChangeType(fieldValue, filterValueType));
            }
        }

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            ObjectListRecordFilter objectListRecordFilter = recordFilter as ObjectListRecordFilter;
            if (objectListRecordFilter == null)
                throw new NullReferenceException("objectListRecordFilter");
            if (fieldValue == null)
                return objectListRecordFilter.CompareOperator == ListRecordFilterOperator.NotIn;

            Type type = GetNonNullableRuntimeType();
            bool isValueInFilter;
            if (type.FullName == "System.Guid")
            {
                isValueInFilter = objectListRecordFilter.Values.Contains(fieldValue.ToString());
            }
            else
            {
                var convertedFieldValue = Convert.ChangeType(fieldValue, type);
                isValueInFilter = objectListRecordFilter.Values.Select(itm => Convert.ChangeType(itm, type)).Contains(convertedFieldValue);
            }

            return objectListRecordFilter.CompareOperator == ListRecordFilterOperator.In ? isValueInFilter : !isValueInFilter;
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            List<Object> nonNullValues = new List<object>();
            if (filterValues != null)
                nonNullValues.AddRange(filterValues.Where(itm => itm != null));

            if (nonNullValues.Count > 0)
            {
                return new ObjectListRecordFilter
                {
                    CompareOperator = ListRecordFilterOperator.In,
                    Values = nonNullValues,
                    FieldName = fieldName
                };
            }
            else
            {
                return new EmptyRecordFilter { FieldName = fieldName };
            }
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
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
    }
}
