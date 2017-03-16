using System;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Notification;
using Vanrise.Notification.Entities;

namespace Vanrise.Analytic.Business
{
    public class DAProfCalcNotificationTypeFilter : IVRNotificationTypeFilter
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }
        public bool IsMatched(IVRNotificationTypeFilterContext context)
        {
            if (context.VRNotificationType.Settings == null)
                return false;

            VRNotificationTypeSettings vrNotificationTypeSettings = context.VRNotificationType.Settings as VRNotificationTypeSettings;
            if (vrNotificationTypeSettings == null)
                return false;

            if (vrNotificationTypeSettings.ExtendedSettings == null)
                return false;

            DataRecordNotificationTypeSettings dataRecordNotificationTypeSettings = vrNotificationTypeSettings.ExtendedSettings as DataRecordNotificationTypeSettings;
            if (dataRecordNotificationTypeSettings == null)
                return false;

            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            DAProfCalcOutputSettings daProfCalcOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<DAProfCalcOutputSettings>(DataAnalysisItemDefinitionId);

            if (daProfCalcOutputSettings.RecordTypeId != dataRecordNotificationTypeSettings.DataRecordTypeId)
                return false;

            return true;
        }
    }
}
