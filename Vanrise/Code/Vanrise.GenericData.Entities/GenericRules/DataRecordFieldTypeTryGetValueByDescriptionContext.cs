using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities.GenericRules
{
    public class DataRecordFieldTypeTryGetValueByDescriptionContext : IDataRecordFieldTypeTryGetValueByDescriptionContext
    {
        public Object FieldDescription { get; set; }
        public List<AdditionalField> AdditionalFields { get; set; }
        public string ErrorMessage { get; set; }
        public Object FieldValue { get; set; }
    }
}
