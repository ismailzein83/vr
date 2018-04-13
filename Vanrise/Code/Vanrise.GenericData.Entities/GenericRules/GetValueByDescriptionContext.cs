using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities.GenericRules
{
    public class GetValueByDescriptionContext : IGetValueByDescriptionContext
    {
        public Object FieldDescription { get; set; }

        public List<AdditionalField> AdditionalFields { get; set; }

        public string ErrorMessage { get; set; }

        public Object FieldValue { get; set; }

        public DataRecordFieldType FieldType { get; set; }

    }
}
