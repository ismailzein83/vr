using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class SummaryTransformationDefinition
    {
        public int SummaryTransformationDefinitionId { get; set; }

        public string Name { get; set; }

        public int RawItemRecordTypeId { get; set; }

        public int SummaryItemRecordTypeId { get; set; }

        public string RawTimeFieldName { get; set; }

        public string SummaryIdFieldName { get; set; }

        public string SummaryBatchStartFieldName { get; set; }

        public SummaryTransformationBatchRangeRetrieval BatchRangeRetrieval { get; set; }

        public List<SummaryTransformationKeyFieldMapping> KeyFieldMappings { get; set; }

        public UpdateSummaryFromRawSettings SummaryFromRawSettings { get; set; }

        public UpdateExistingSummaryFromNewSettings UpdateExistingSummaryFromNewSettings { get; set; }

        public int DataRecordStorageId { get; set; }
    }

    public abstract class SummaryTransformationBatchRangeRetrieval
    {
        public abstract Guid ConfigId { get; }

        public abstract void GetRawItemBatchTimeRange(dynamic rawItem, DateTime rawItemTime, out DateTime batchStart);
    }

    public class SummaryTransformationKeyFieldMapping
    {
        public string RawFieldName { get; set; }

        /// <summary>
        /// either RawFieldName or GetRawFieldExpression should have value. RawFieldName has more priority than GetRawFieldExpression
        /// </summary>
        public string GetRawFieldExpression { get; set; }

        public string SummaryFieldName { get; set; }
    }

    public class UpdateSummaryFromRawSettings
    {
        public int TransformationDefinitionId { get; set; }

        public string RawRecordName { get; set; }

        public string SymmaryRecordName { get; set; }
    }

    public class UpdateExistingSummaryFromNewSettings
    {
        public int TransformationDefinitionId { get; set; }

        public string ExistingRecordName { get; set; }

        public string NewRecordName { get; set; }
    }
}
