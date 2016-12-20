using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class GenericFieldDefinitionInfo
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; set; }
    }
}
