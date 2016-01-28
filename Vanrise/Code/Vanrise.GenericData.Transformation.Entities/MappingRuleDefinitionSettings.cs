using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Transformation.Entities
{
    public class MappingRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public string FieldTitle { get; set; }

        public DataRecordFieldType FieldType { get; set; }
    }
}
