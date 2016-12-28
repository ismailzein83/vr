﻿using System;
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

        public Guid BusinessEntityDefinitionId { get; set; }
        public bool IsNullable { get; set; }
        public Object SelectorFilter { get; set; }
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

            BusinessEntityDefinition beDefinition = GetBusinessEntityDefinition();
            var beManager = GetBusinessEntityManager();
            var entityDescriptions = new List<string>();

            IEnumerable<object> valueAsList = FieldTypeHelper.ConvertFieldValueToList<object>(value);
            if (valueAsList != null)
            {
                if (valueAsList.Count() == 0)
                    return null;
                else
                {
                    foreach (var entityId in valueAsList)
                    {
                        entityDescriptions.Add(beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityId = entityId }));
                    }
                }
            }
            else
            {
                entityDescriptions.Add(beManager.GetEntityDescription(new BusinessEntityDescriptionContext() { EntityDefinition = beDefinition, EntityId = value }));
            }
            return String.Join(",", entityDescriptions);
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
                throw new NullReferenceException("stringListRecordFilter");
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
            return new ObjectListRecordFilter
            {
                CompareOperator = ListRecordFilterOperator.In,
                Values = filterValues.Select(value => value).ToList(),
                FieldName = fieldName
            };
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

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override string GetFilterDescription(RecordFilter filter)
        {
            ObjectListRecordFilter objectListRecordFilter = filter as ObjectListRecordFilter;
            return string.Format(" {0} {1} ( {2} ) ", objectListRecordFilter.FieldName, Utilities.GetEnumDescription(objectListRecordFilter.CompareOperator), GetDescription(objectListRecordFilter.Values.Cast<Object>().ToList()));
        }
    }
}
