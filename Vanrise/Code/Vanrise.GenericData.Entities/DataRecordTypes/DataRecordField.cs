using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordField
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public DataRecordFieldType Type { get; set; }

        public DataRecordFieldFormula Formula { get; set; }

        public bool IsInheritedFromExtraField { get; set; }
    }

    public class DataRecordGridColumnAttribute
    {
        public GridColumnAttribute Attribute { get; set; }
 
        public string Name { get; set; }
        
        public string DetailViewerEditor { get; set; }
    }
}