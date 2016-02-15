using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueActivatorExecutionContext : IQueueActivatorExecutionContext
    {
        PersistentQueueItem _itemToProcess;
        QueueInstance _queueInstance;
        public QueueActivatorExecutionContext(PersistentQueueItem itemToProcess, QueueInstance queueInstance)
        {
            this._itemToProcess = itemToProcess;
            this._queueInstance = queueInstance;
        }

        public PersistentQueueItem ItemToProcess
        {
            get
            {
                return this._itemToProcess;
            }
        }

        ItemsToEnqueue _outputItems = new ItemsToEnqueue();
        public ItemsToEnqueue OutputItems
        {
            get { return _outputItems; }
        }

        public QueueExecutionFlowStage CurrentStage
        {
            get
            {
                return GetStage(this._queueInstance.StageName);
            }        
        }


        QueueExecutionFlowStagesByStageName _queueExecutionFlowStagesByStageName;

        public QueueExecutionFlowStage GetStage(string stageName)
        {
            if(_queueExecutionFlowStagesByStageName == null)
            {
                QueueExecutionFlowManager execFlowManager = new QueueExecutionFlowManager();
                var execFlow = execFlowManager.GetExecutionFlow(this._queueInstance.ExecutionFlowId.Value);
                if(execFlow == null)
                    throw new NullReferenceException(String.Format("Execution Flow '{0}'", this._queueInstance.ExecutionFlowId.Value));
                QueueExecutionFlowDefinitionManager execFlowDefinitionManager = new QueueExecutionFlowDefinitionManager();
                _queueExecutionFlowStagesByStageName = execFlowDefinitionManager.GetFlowStages(execFlow.DefinitionId);
                if (_queueExecutionFlowStagesByStageName == null)
                    throw new NullReferenceException(String.Format("Execution Flow Stages '{0}'", execFlow.DefinitionId));
            }
            QueueExecutionFlowStage stage;
            if (_queueExecutionFlowStagesByStageName.TryGetValue(stageName, out stage))
                return stage;
            else
                throw new NullReferenceException(String.Format("Stage '{0}' in Execution Flow '{1}'", stageName, this._queueInstance.ExecutionFlowId.Value));
        }

        public QueueInstance Queue
        {
            get
            {
                return this._queueInstance;
            }
        }
    }
}
