using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class UpdateGenericBEFieldRuntime
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public DataRecordFieldType Type { get; set; }
        public string RuntimeEditor { get; set; }
        public Boolean IsRequired { get; set; }
        public object DefaultValue { get; set; }
        public UpdateGenericBEFieldState FieldState { get; set; }
    }
}
