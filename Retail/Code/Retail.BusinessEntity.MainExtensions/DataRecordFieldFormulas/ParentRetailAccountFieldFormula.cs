using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class ParentRetailAccountFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("B3D9B0A4-B751-4544-8A7A-6764687059ED"); } }

        public string AccountTypeFieldName { get; set; }

        public string AccountFieldName { get; set; }

        public Guid ParentAccountTypeId { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { AccountTypeFieldName, AccountFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic accountId = context.GetFieldValue(AccountFieldName);
            if (accountId == null)
                return null;

            FieldBusinessEntityType accountFieldType = context.FieldType as FieldBusinessEntityType;
            accountFieldType.ThrowIfNull("accountFieldType");

            Account account = new AccountBEManager().GetSelfOrParentAccountOfType(accountFieldType.BusinessEntityDefinitionId, accountId, ParentAccountTypeId);
            if (account == null)
                return null;

            return account.AccountId;
        }

        public override RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            context.InitialFilter.ThrowIfNull("context.InitialFilter");

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                FieldBusinessEntityType accountFieldType = context.FieldType as FieldBusinessEntityType;
                accountFieldType.ThrowIfNull("accountFieldType");

                AccountBEManager accountBEManager = new AccountBEManager();
                List<long> parentAndChildsAccountIds = new List<long>();

                foreach (var parentAccountId in objectListFilter.Values)
                {
                    long parsedParentAccountId = Convert.ToInt64(parentAccountId);
                    parentAndChildsAccountIds.Add(parsedParentAccountId);

                    List<long> childAccountIds = accountBEManager.GetChildAccountIds(accountFieldType.BusinessEntityDefinitionId, parsedParentAccountId, true);
                    if (childAccountIds != null)
                        parentAndChildsAccountIds.AddRange(childAccountIds);
                }

                List<object> filterValues = parentAndChildsAccountIds.Count > 0 ? parentAndChildsAccountIds.Cast<Object>().ToList() : null;

                var childFilter = accountFieldType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = this.AccountFieldName, FilterValues = filterValues, StrictEqual = true });
                if (childFilter is ObjectListRecordFilter)
                    ((ObjectListRecordFilter)childFilter).CompareOperator = objectListFilter.CompareOperator;
                return childFilter;
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                EmptyRecordFilter accountTypeEmptyRecordFilter = new EmptyRecordFilter { FieldName = AccountFieldName };

                ObjectListRecordFilter parentAccountTypeRecordFilter = new ObjectListRecordFilter();
                parentAccountTypeRecordFilter.FieldName = AccountTypeFieldName;
                parentAccountTypeRecordFilter.CompareOperator = ListRecordFilterOperator.NotIn;
                parentAccountTypeRecordFilter.Values = new List<object>();

                HashSet<Guid> childrenAccountTypeIds = new AccountTypeManager().GetSelfAndSupportedChildrenAccountTypeIds(ParentAccountTypeId);
                if (childrenAccountTypeIds != null && childrenAccountTypeIds.Count > 0)
                    parentAccountTypeRecordFilter.Values.AddRange(childrenAccountTypeIds.Cast<object>());

                return new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.Or,
                    Filters = new List<RecordFilter>() { parentAccountTypeRecordFilter, accountTypeEmptyRecordFilter }
                };
            }

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                NonEmptyRecordFilter accountTypeNonEmptyRecordFilter = new NonEmptyRecordFilter { FieldName = AccountFieldName };

                ObjectListRecordFilter parentAccountTypeRecordFilter = new ObjectListRecordFilter();
                parentAccountTypeRecordFilter.FieldName = AccountTypeFieldName;
                parentAccountTypeRecordFilter.CompareOperator = ListRecordFilterOperator.In;
                parentAccountTypeRecordFilter.Values = new List<object>();

                HashSet<Guid> childrenAccountTypeIds = new AccountTypeManager().GetSelfAndSupportedChildrenAccountTypeIds(ParentAccountTypeId);
                if (childrenAccountTypeIds != null && childrenAccountTypeIds.Count > 0)
                    parentAccountTypeRecordFilter.Values.AddRange(childrenAccountTypeIds.Cast<object>());

                return new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>() { parentAccountTypeRecordFilter, accountTypeNonEmptyRecordFilter }
                };
            }

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }
    }
}
