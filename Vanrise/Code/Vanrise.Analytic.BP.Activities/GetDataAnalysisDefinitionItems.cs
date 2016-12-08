using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using Vanrise.BusinessProcess;

namespace Vanrise.Analytic.BP.Activities
{
    public sealed class GetDataAnalysisDefinitionItems : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> DataAnalysisDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<List<Guid>> DataAnalysisItemDefinitionIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid dataAnalysisDefinitionId = this.DataAnalysisDefinitionId.Get(context);
            List<Guid> dataAnalysisItemDefinitionIds = new List<Guid>();

            DataAnalysisItemDefinitionManager manager = new DataAnalysisItemDefinitionManager();
            IEnumerable<DataAnalysisItemDefinition> dataAnalysisItemDefinitions = manager.GetDataAnalysisItemDefinitionsById(dataAnalysisDefinitionId);

            if (dataAnalysisItemDefinitions == null)
                throw new NullReferenceException("dataAnalysisItemDefinitions");

            foreach (DataAnalysisItemDefinition dataAnalysisItemDefinition in dataAnalysisItemDefinitions)
            {
                dataAnalysisItemDefinitionIds.Add(dataAnalysisItemDefinition.DataAnalysisItemDefinitionId);
            }

            DataAnalysisItemDefinitionIds.Set(context, dataAnalysisItemDefinitionIds);

            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Data Analysis Definition Items loaded.", null);
        }
    }
}