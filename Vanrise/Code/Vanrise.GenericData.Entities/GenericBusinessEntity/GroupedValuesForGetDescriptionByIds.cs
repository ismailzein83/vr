using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class GroupedValuesForGetDescriptionByIds
    {
        public DataRecordFieldType FieldType { get; set; }

        public Dictionary<object, List<ISetRecordDescription>> RecordsByValue { get; set; }
    }
    #region To Be Deleted
    public class GetDescriptionByIdsDicValueOutput
    {
        public DataRecordFieldType FieldType { get; set; }

        public List<object> Values { get; set; }

        public Dictionary<object, List<ISetRecordDescription>> Descriptions { get; set; }

        public void SetDescription(Dictionary<object, string> descriptionsByFieldValue)
        {
            foreach (var kvp in descriptionsByFieldValue)
            {
                var fieldValue = kvp.Key;
                var description = kvp.Value;
                List<ISetRecordDescription> recordCollectedValues = null;
                if (Descriptions.TryGetValue(fieldValue, out recordCollectedValues))
                {
                    foreach (var recordCollectedValue in recordCollectedValues)
                    {
                        recordCollectedValue.SetDescription(new SetRecordDescriptionContext { Description = description });
                    }
                }
            }
        }
    }
    #endregion
}