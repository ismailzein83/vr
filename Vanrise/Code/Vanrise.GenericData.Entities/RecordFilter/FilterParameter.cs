using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class FilterParameter
    {
        public Guid FilterParameterId { get; set; }

        public string Title { get; set; }

        public DataRecordFieldType FieldType { get; set; }

        public dynamic DefaultValue { get; set; }
    }

    public class FilterParameterCollection : List<FilterParameter>
    {

    }
}
