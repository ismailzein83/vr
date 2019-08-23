using System;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordFieldValue : ISetRecordDescription
    {
        public Object Value { get; set; }

        public string Description { get; set; }

        public void SetDescription(ISetRecordDescriptionContext context)
        {
            Description = context.Description;
        }
    }
}