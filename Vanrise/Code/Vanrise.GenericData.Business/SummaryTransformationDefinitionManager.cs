using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class SummaryTransformationDefinitionManager
    {
        public SummaryTransformationDefinition GetSummaryTransformationDefinition(int summaryTransformationDefinitionId)
        {
            var definition = new SummaryTransformationDefinition
                {
                    SummaryTransformationDefinitionId = 1,
                    Name = "Traffic Summary Transformation",
                    RawItemRecordTypeId = 10,
                    SummaryItemRecordTypeId = 18,
                    DataRecordStorageId = 9,
                    BatchRangeRetrieval = new SummaryBatchTimeInterval
                    {
                        RawTimeFieldName = "AttemptTime",
                        IntervalInMinutes = 10
                    },
                    SummaryIdFieldName = "ID",
                    SummaryBatchStartFieldName = "BatchStart",
                    KeyFieldMappings = new List<SummaryTransformationKeyFieldMapping>
                    {
                        new SummaryTransformationKeyFieldMapping
                        { 
                            RawFieldName= "OperatorAccount", 
                            SummaryFieldName = "OperatorID"
                        }
                    },
                    SummaryFromRawSettings = new UpdateSummaryFromRawSettings
                    {
                        TransformationDefinitionId = 3,
                        RawRecordName = "cdr",
                        SymmaryRecordName = "trafficSummary"
                    },
                    UpdateExistingSummaryFromNewSettings = new UpdateExistingSummaryFromNewSettings
                    {
                        TransformationDefinitionId = 4,
                        ExistingRecordName = "existingItem",
                        NewRecordName = "newItem"
                    }
                };
            return definition;
        }
    }
}
