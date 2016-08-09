using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.ExcelConversion.Entities
{
    public class ListMapping
    {
        public string ListName { get; set; }

        public int SheetIndex { get; set; }

        public int FirstRowIndex { get; set; }

        public int? LastRowIndex { get; set; }

        public List<FieldMapping> FieldMappings { get; set; }

        public MappingFilter Filter { get; set; }
    }

    public class MappingFilter
    {
        public string ConditionExpression { get; set; }
        public List<MappingFilterField> Fields { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
    }
    public class MappingFilterField
    {
        public string FieldName { get; set; }
        public FieldMapping FieldMapping { get; set; }
        public DataRecordFieldType FieldType { get; set; }
    }
}
