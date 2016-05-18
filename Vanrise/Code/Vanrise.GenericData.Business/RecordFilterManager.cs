using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class RecordFilterManager
    {
        public bool IsFilterGroupMatch(RecordFilterGroup filterGroup, IRecordFilterGenericFieldMatchContext context)
        {
            if(filterGroup == null)
                return true;
            if (filterGroup.Filters == null)
                throw new NullReferenceException("filterGroup.Filters");
            foreach(var filter in filterGroup.Filters)
            {
                bool isMatch = IsFilterMatch(filter, context);
                if (isMatch && filterGroup.LogicalOperator == RecordQueryLogicalOperator.Or)
                    return true;
                if (!isMatch && filterGroup.LogicalOperator == RecordQueryLogicalOperator.And)
                    return false;
            }
            return true;
        }

        private bool IsFilterMatch(RecordFilter filter, IRecordFilterGenericFieldMatchContext context)
        {
            RecordFilterGroup filterGroup = filter as RecordFilterGroup;
            if (filterGroup != null)
                return IsFilterGroupMatch(filterGroup, context);

            DataRecordFieldType fieldType;
            var fieldValue = context.GetFieldValue(filter.FieldName, out fieldType);
            
            EmptyRecordFilter emptyFilter = filter as EmptyRecordFilter;
            if (emptyFilter != null)
                return fieldValue == null;

            NonEmptyRecordFilter nonEmptyFilter = filter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
                return fieldValue != null;

            return fieldType.IsMatched(fieldValue, filter);
        }
    }

    public interface IRecordFilterGenericFieldMatchContext
    {
        Object GetFieldValue(string fieldName, out DataRecordFieldType fieldType);
    }
}
