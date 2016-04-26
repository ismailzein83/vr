namespace Vanrise.GenericData.Entities
{
    public class DataRecordFieldTypeConfig
    {
        public int DataRecordFieldTypeConfigId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Editor { get; set; }

        public string RuntimeEditor { get; set; }

        public string FilterEditor { get; set; }

        public bool IsSupportedInGenericRule { get; set; }

        public string RuleFilterEditor { get; set; }
    }
}
