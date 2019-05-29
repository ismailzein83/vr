﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldBusinessEntityType : DataRecordFieldType, IFieldBusinessEntityType
    {
        public override Guid ConfigId { get { return new Guid("2e16c3d4-837b-4433-b80e-7c02f6d71467"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-businessentity-runtimeeditor"; } }

        public override string ViewerEditor { get { return "vr-genericdata-fieldtype-businessentity-viewereditor"; } }

        public override DataRecordFieldOrderType OrderType { get { return DataRecordFieldOrderType.ByFieldDescription; } }

        public Guid BusinessEntityDefinitionId { get; set; }

        public bool IsNullable { get; set; }

        public BERuntimeSelectorFilter BERuntimeSelectorFilter { get; set; }

        public List<FieldBusinessEntityTypeDependantField> DependantFields { get; set; }

        #region Public Methods

        Type _runtimeType;
        public override Type GetRuntimeType()
        {
            if (_runtimeType == null)
            {
                var type = GetNonNullableRuntimeType();
                lock (this)
                {
                    if (_runtimeType == null)
                        _runtimeType = (IsNullable) ? GetNullableType(type) : type;
                }
            }
            return _runtimeType;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            if (newValue == null && oldValue == null)
                return true;

            if (newValue == null || oldValue == null)
                return false;

            var type = GetNonNullableRuntimeType();
            if (type == typeof(Guid))
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

        Type _nonNullableRuntimeType;
        public override Type GetNonNullableRuntimeType()
        {
            if (_nonNullableRuntimeType == null)
            {
                BusinessEntityDefinition beDefinition = GetBusinessEntityDefinition();
                lock (this)
                {
                    if (_nonNullableRuntimeType == null)
                    {

                        if (beDefinition.Settings.IdType == null)
                            throw new NullReferenceException("beDefinition.Settings.IdType");
                        _nonNullableRuntimeType = Type.GetType(beDefinition.Settings.IdType);
                    }
                }

            }
            return _nonNullableRuntimeType;
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
                            if (takenCount > 10)
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

        public override RecordFilter ConvertToRecordFilter(IDataRecordFieldTypeConvertToRecordFilterContext context)
        {
            if (context.FilterValues == null || context.FilterValues.Count == 0)
                return null;

            List<Object> nonNullValues = new List<object>();
            if (context.FilterValues != null)
                nonNullValues.AddRange(context.FilterValues.Where(itm => itm != null));

            if (nonNullValues.Count > 0)
            {
                return new ObjectListRecordFilter
                {
                    CompareOperator = ListRecordFilterOperator.In,
                    Values = nonNullValues,
                    FieldName = context.FieldName
                };
            }
            else
            {
                return new EmptyRecordFilter { FieldName = context.FieldName };
            }
        }

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        protected override dynamic ParseNonNullValueToFieldType(Object originalValue)
        {
            var type = GetNonNullableRuntimeType();
            if (type.Equals(typeof(Guid)))
                return FieldGuidType.ParseNonNullValueToGuid(originalValue);
            else
                return Convert.ChangeType(originalValue, type);
        }

        public override void GetValueByDescription(IGetValueByDescriptionContext context)
        {
            if (context.FieldDescription == null)
                return;
            var businessEntityContext = new BusinessEntityGetIdByDescriptionContext
            {
                FieldDescription = context.FieldDescription,
                FieldType = context.FieldType,
                BusinessEntityDefinitionId = BusinessEntityDefinitionId
            };

            var beManager = GetBusinessEntityManager();
            beManager.GetIdByDescription(businessEntityContext);
            if (businessEntityContext.ErrorMessage != null)
                context.ErrorMessage = businessEntityContext.ErrorMessage;
            else
                context.FieldValue = businessEntityContext.FieldValue;

        }

        public override bool RenderDescriptionByDefault()
        {
            return true;
        }

        public override string GetRuntimeTypeDescription()
        {
            BusinessEntityDefinitionManager _manager = new BusinessEntityDefinitionManager();
            return _manager.GetBusinessEntityDefinitionName(BusinessEntityDefinitionId);
        }

        public override bool IsStillAvailable(IDataRecordFieldTypeIsStillAvailableContext context)
        {
            BaseBusinessEntityManager baseBusinessEntityManager = GetBusinessEntityManager();
            return baseBusinessEntityManager.IsStillAvailable(new BusinessEntityIsStillAvailableContext() { EntityId = context.EntityId });
        }

        public override bool IsCompatibleWithFieldType(DataRecordFieldType fieldType)
        {
            FieldBusinessEntityType fieldTypeAsBusinessEntityType = fieldType as FieldBusinessEntityType;
            if (fieldTypeAsBusinessEntityType == null)
                return false;
            if (fieldTypeAsBusinessEntityType.BusinessEntityDefinitionId != this.BusinessEntityDefinitionId)
                return false;

            if (fieldTypeAsBusinessEntityType.BERuntimeSelectorFilter == null && this.BERuntimeSelectorFilter == null)
                return true;
            else if (Vanrise.Common.Serializer.Serialize(fieldTypeAsBusinessEntityType.BERuntimeSelectorFilter) == Vanrise.Common.Serializer.Serialize(this.BERuntimeSelectorFilter))
                return true;
            else
                return false;
        }

        public override RDBDataRecordFieldAttribute GetDefaultRDBFieldAttribute(IDataRecordFieldTypeDefaultRDBFieldAttributeContext context)
        {
            RDBDataRecordFieldAttribute rDBDataRecordFieldAttribute = new RDBDataRecordFieldAttribute();

            switch (GetBusinessEntityDefinition().Settings.IdType)
            {
                case "System.Int64":
                    rDBDataRecordFieldAttribute.RdbDataType = RDBDataType.BigInt;
                    break;
                case "System.Int32":
                    rDBDataRecordFieldAttribute.RdbDataType = RDBDataType.Int;
                    break;
                case "System.Guid":
                    rDBDataRecordFieldAttribute.RdbDataType = RDBDataType.UniqueIdentifier;
                    break;

                default:
                    throw new NotSupportedException(String.Format("Field Business Entity Type"));
            }

            return rDBDataRecordFieldAttribute;
        }

        public override void SetDependentFieldValues(ISetDependentFieldValuesContext context)
        {
            if (this.DependantFields == null || this.DependantFields.Count == 0)
                return;

            if (context.FieldValue == null)
                return;

            Entities.GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntityManager().GetGenericBusinessEntity(context.FieldValue, this.BusinessEntityDefinitionId);
            genericBusinessEntity.ThrowIfNull("genericBusinessEntity", context.FieldValue as object);

            foreach (var dependantField in this.DependantFields)
            {
                object dependantFieldValue;
                if (!genericBusinessEntity.FieldValues.TryGetValue(dependantField.MappedFieldName, out dependantFieldValue))
                    continue;

                if (!context.DependentFieldValues.ContainsKey(dependantField.FieldName))
                    context.DependentFieldValues.Add(dependantField.FieldName, dependantFieldValue);

                DataRecordField dependantFieldType;
                if (!context.DataRecordFields.TryGetValue(dependantField.FieldName, out dependantFieldType))
                    throw new Exception($"context.DataRecordFields does not contain {dependantField.FieldName} in DataRecordType Id : {context.DataRecordTypeId}");

                SetDependentFieldValuesContext setDependentFieldValuesContext = new SetDependentFieldValuesContext()
                {
                    FieldName = dependantField.FieldName,
                    FieldValue = dependantFieldValue,
                    DataRecordTypeId = context.DataRecordTypeId,
                    DataRecordFields = context.DataRecordFields,
                    DependentFieldValues = context.DependentFieldValues
                };
                dependantFieldType.Type.SetDependentFieldValues(setDependentFieldValuesContext);
            }
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

        BaseBusinessEntityManager GetBusinessEntityManager()
        {
            var beDefinitionManager = new BusinessEntityDefinitionManager();
            var beManagerInstance = beDefinitionManager.GetBusinessEntityManager(this.BusinessEntityDefinitionId);
            if (beManagerInstance == null)
                throw new NullReferenceException(String.Format("beManagerInstance. BusinessEntityDefinitionId '{0}'", this.BusinessEntityDefinitionId));

            return beManagerInstance;
        }

        #endregion
    }

    public class FieldBusinessEntityTypeDependantField
    {
        public string FieldName { get; set; }

        public string MappedFieldName { get; set; }

        public bool IsRequired { get; set; }
    }
}