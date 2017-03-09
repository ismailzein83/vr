using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcActionTargetType : Vanrise.GenericData.Notification.DataRecordActionTargetType
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }
        public override Guid DataRecordTypeId
        {
            get
            {
                DataAnalysisItemDefinitionManager manager = new DataAnalysisItemDefinitionManager();
                return manager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(DataAnalysisItemDefinitionId).RecordTypeId;
            }
        }
    }
}