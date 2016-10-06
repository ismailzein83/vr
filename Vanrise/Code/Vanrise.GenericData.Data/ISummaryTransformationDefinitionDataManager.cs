using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data
{
    public interface ISummaryTransformationDefinitionDataManager : IDataManager
    {

        List<SummaryTransformationDefinition> GetSummaryTransformationDefinitions();

        bool AreSummaryTransformationDefinitionsUpdated(ref object updateHandle);

        bool UpdateSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition);

        bool AddSummaryTransformationDefinition(SummaryTransformationDefinition summaryTransformationDefinition);

    }
}

