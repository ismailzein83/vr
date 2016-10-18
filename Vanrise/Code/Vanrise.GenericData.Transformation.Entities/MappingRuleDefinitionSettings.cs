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
        public override Guid ConfigId
        {
            get { return new Guid("ae91755c-f573-4add-8dba-7733193384af"); }
        }
        public string FieldTitle { get; set; }

        public DataRecordFieldType FieldType { get; set; }


    }
}
