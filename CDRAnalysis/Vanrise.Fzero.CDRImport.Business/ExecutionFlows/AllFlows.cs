using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Business.ExecutionFlows
{
    public static class AllFlows
    {
        public static string GetNormalCDRFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {                    
                    new QueueStageExecutionActivity { StageName = "CDR Import",  QueueName = "CDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                        QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(CDRImportActivator).AssemblyQualifiedName} }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetNormalCDRNormalizationFlow()
        {
            QueueExecutionFlowTree queueFlowTree = new QueueExecutionFlowTree
            {
                Activities = new List<BaseExecutionActivity>
                {    
                     new QueueStageExecutionActivity { StageName = "Normalize CDRs",  QueueName = "NormalizeCDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(CDRNormalizationActivator).AssemblyQualifiedName} },
                    new QueueStageExecutionActivity { StageName = "CDR Import",  QueueName = "CDRQueue", QueueTypeFQTN = typeof(ImportedCDRBatch).AssemblyQualifiedName,
                        QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(CDRImportActivator).AssemblyQualifiedName} }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetStagingCDRNormalizationFlow()
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
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StagingCDRNormalizationActivator).AssemblyQualifiedName} },
                            new QueueStageExecutionActivity { StageName = "Save CDRs",  QueueName = "StoreCDRQueue", QueueTypeFQTN = typeof(ImportedStagingCDRBatch).AssemblyQualifiedName,
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StagingCDRImportActivator).AssemblyQualifiedName} }
                        }
                    }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }

        public static string GetStagingCDRFlow()
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
                                QueueSettings = new QueueSettings { QueueActivatorFQTN = typeof(StagingCDRImportActivator).AssemblyQualifiedName} }
                        }
                    }
                }
            };
            return Vanrise.Common.Serializer.Serialize(queueFlowTree);
        }
    }
}
