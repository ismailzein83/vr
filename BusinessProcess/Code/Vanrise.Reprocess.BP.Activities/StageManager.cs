using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class StageManager
    {
        Dictionary<string, ReprocessStage> _reprocessingStagesByName = new Dictionary<string, ReprocessStage>();
        List<string> _initiationStageNames;
        public StageManager(ReprocessDefinition reprocessDefinition)
        {
            if (reprocessDefinition == null)
                throw new ArgumentNullException("reprocessDefinition");

            var stages = new QueueExecutionFlowDefinitionManager().GetFlowStages(reprocessDefinition.ExecutionFlowDefinitionId);
            if (stages == null)
                throw new NullReferenceException(String.Format("stages '{0}'", reprocessDefinition.ExecutionFlowDefinitionId));
            foreach (var stage in stages.Values)
            {
                var reprocessActivator = stage.QueueActivator as IReprocessStageActivator;
                if (reprocessActivator != null)
                {
                    var reprocessStage = new ReprocessStage
                    {
                        StageName = stage.StageName,
                        Activator = reprocessActivator,
                        PreviousStageNames = new List<string>(), 
                        StageQueue = reprocessActivator.GetQueue(),
                        Status = new AsyncActivityStatus(),
                        PreviousStatus = new AsyncActivityStatus(),
                        SubscribedStageNames = new List<string>()
                    };
                    if (stage.SourceStages != null)
                        reprocessStage.PreviousStageNames.AddRange(stage.SourceStages);
                    FillSubscribedStageNames(reprocessStage.SubscribedStageNames, stage.StageName, stage.StageName, stages.Values);
                    _reprocessingStagesByName.Add(stage.StageName, reprocessStage);
                }
            }
            foreach(var reprocessStage in _reprocessingStagesByName.Values)
            {
                var outputStages = reprocessStage.Activator.GetOutputStages();
                if(outputStages != null)
                {
                    foreach(var outputStageName in outputStages)
                    {
                        GetStage(outputStageName).PreviousStageNames.Add(reprocessStage.StageName);
                    }
                }
            }
            if (reprocessDefinition.InitiationStageNames == null)
                throw new NullReferenceException(String.Format("reprocessDefinition.SourceStageNames. ReprocessDefinitionId '{0}'", reprocessDefinition.ReprocessDefinitionId));
            _initiationStageNames = reprocessDefinition.InitiationStageNames;
        }

        private void FillSubscribedStageNames(List<string> subscribedStageNames, string sourceStageName, string mainStageName, IEnumerable<QueueExecutionFlowStage> stages)
        {
            foreach(var stage in stages)
            {
                if(stage.StageName != mainStageName && !subscribedStageNames.Contains(stage.StageName))
                {
                    if(stage.SourceStages != null && stage.SourceStages.Contains(sourceStageName))
                    {
                        subscribedStageNames.Add(stage.StageName);
                        FillSubscribedStageNames(subscribedStageNames, stage.StageName, mainStageName, stages);
                    }
                }
            }
        }

        public BaseQueue<IReprocessBatch> GetStageQueue(string stageName)
        {
            ReprocessStage stage = GetStage(stageName);
            return stage.StageQueue;
        }

        public void EnqueueBatch(string stageName, IReprocessBatch batch)
        {
            var stage = GetStage(stageName);
            stage.StageQueue.Enqueue(batch);
            if(stage.SubscribedStageNames != null)
            {
                foreach(var subscribedStageName in stage.SubscribedStageNames)
                {
                    GetStage(subscribedStageName).StageQueue.Enqueue(batch);
                }
            }
        }

        private ReprocessStage GetStage(string stageName)
        {
            ReprocessStage stage;
            if (!_reprocessingStagesByName.TryGetValue(stageName, out stage))
                throw new Exception(String.Format("Stage '{0}' is not available", stageName));
            return stage;
        }

        public IEnumerable<ReprocessStage> GetReprocessingStages()
        {
            return _reprocessingStagesByName.Values;
        }

        internal void EvaluateStagesStatus(AsyncActivityStatus loadDataToReprocessStatus)
        {
            foreach(var stage in GetReprocessingStages())
            {
                if (stage.PreviousStatus.IsComplete)
                    continue;
                if (!loadDataToReprocessStatus.IsComplete && _initiationStageNames.Contains(stage.StageName))//in case the load Data activity push data to this stage
                    continue;
                if (stage.PreviousStageNames != null && stage.PreviousStageNames.Any(previousStageName => !GetStage(previousStageName).Status.IsComplete))//if any previous stage is still running
                    continue;
                stage.PreviousStatus.IsComplete = true;
            }
        }
    }

    public class ReprocessStage
    {
        public string StageName { get; set; }

        public IReprocessStageActivator Activator { get; set; }

        public BaseQueue<IReprocessBatch> StageQueue { get; set; }

        public AsyncActivityStatus Status { get; set; }

        public AsyncActivityStatus PreviousStatus { get; set; }

        public List<string> PreviousStageNames { get; set; }

        public List<string> SubscribedStageNames { get; set; }
    }
}
