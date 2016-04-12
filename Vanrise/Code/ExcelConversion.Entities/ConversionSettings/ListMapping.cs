using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ListMapping
    {
        public string ListName { get; set; }

        public int SheetIndex { get; set; }

        public int FirstRowIndex { get; set; }

        public int? LastRowIndex { get; set; }

        public List<FieldMapping> FieldMappings { get; set; }

        public RecordFilter Filter { get; set; }
    }

    public class RecordFilter
    {

    }
}
