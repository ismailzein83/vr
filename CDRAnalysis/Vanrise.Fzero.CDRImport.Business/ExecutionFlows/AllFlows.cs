using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.CDRImport.QueueActivators;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business.ExecutionFlows
{
    public static class AllFlows
    {
        public static string GetImporttoNormalCDRFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {                    
                    new QueueStageExecutionActivity { StageName = "CDR Import",  QueueName = "CDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                        QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(SaveCDRActivator).AssemblyQualifiedName} }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetNormalize_ImporttoNormalCDRFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {    
                     new QueueStageExecutionActivity { StageName = "Normalize CDRs",  QueueName = "NormalizeCDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(NormalizeCDRActivator).AssemblyQualifiedName} },
                    new QueueStageExecutionActivity { StageName = "CDR Import",  QueueName = "CDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                        QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(SaveCDRActivator).AssemblyQualifiedName} }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetNormalize_ImporttoStagingCDRFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {
                    new SequenceExecutionActivity
                    {
                        Activities = new List<BaseExecutionActivity>
                        {     
                            new QueueStageExecutionActivity { StageName = "Normalize CDRs",  QueueName = "NormalizeCDRQueue", QueueTypeFQTN = typeof(ImportedStagingCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(NormalizeStagingCDRActivator).AssemblyQualifiedName} },
                            new QueueStageExecutionActivity { StageName = "Save CDRs",  QueueName = "StoreCDRQueue", QueueTypeFQTN = typeof(ImportedStagingCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(SaveStagingCDRActivator).AssemblyQualifiedName} }
                        }
                    }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetImporttoStagingCDRFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {
                    new SequenceExecutionActivity
                    {
                        Activities = new List<BaseExecutionActivity>
                        {     
                            new QueueStageExecutionActivity { StageName = "Save CDRs",  QueueName = "StoreCDRQueue", QueueTypeFQTN = typeof(ImportedStagingCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(SaveStagingCDRActivator).AssemblyQualifiedName} }
                        }
                    }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }
    }
}
