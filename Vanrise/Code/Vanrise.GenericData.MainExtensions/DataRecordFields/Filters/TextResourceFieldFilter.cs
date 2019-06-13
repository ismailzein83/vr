using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields.Filters
{
    public class TextResourceFieldFilter : IDataRecordFieldFilter
    {
        public bool IsExcluded(IDataRecordFieldFilterContext context)
        {
            if (context.DataRecordField == null || context.DataRecordField.Type == null)
                return true;

            if (!(context.DataRecordField.Type is FieldTextResourceType))
                return true;

            return false;
        }
    }
}
