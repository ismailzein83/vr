using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordFieldType
    {
        public int ConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public DataRecordFieldTypeSettings Settings { get; set; }

        public abstract Type GetRuntimeType();
    }

    public class DataRecordFieldTypeSettings
    {
        public string Editor { get; set; }

        public string DynamicGroupUIControl { get; set; }

        public string SelectorControl { get; set; }

        public bool IsSupportedInGenericRule { get; set; }
    }
}
