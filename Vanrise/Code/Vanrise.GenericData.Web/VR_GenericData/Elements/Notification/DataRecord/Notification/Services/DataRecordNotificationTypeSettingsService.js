(function (appControllers) {

    'use strict';

    DataRecordNotificationTypeSettingsService.$inject = ['VRModalService', 'VR_Notification_NotificationStatusEnum', 'LabelColorsEnum', ];

    function DataRecordNotificationTypeSettingsService(VRModalService, VR_Notification_NotificationStatusEnum, LabelColorsEnum) {

        function getStatusColor(status) {

            if (status === VR_Notification_NotificationStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === VR_Notification_NotificationStatusEnum.Executing.value) return LabelColorsEnum.Info.color;
            if (status === VR_Notification_NotificationStatusEnum.Executed.value) return LabelColorsEnum.Success.color;
            if (status === VR_Notification_NotificationStatusEnum.RolledBack.value) return LabelColorsEnum.Warning.color;
            if (status === VR_Notification_NotificationStatusEnum.ErrorOnExecution.value) return LabelColorsEnum.Error.color;
            if (status === VR_Notification_NotificationStatusEnum.ErrorOnRollback.value) return LabelColorsEnum.Error.color;
            if (status === VR_Notification_NotificationStatusEnum.Rollback.value) return LabelColorsEnum.Info.color;

            return LabelColorsEnum.Info.color;
        };

        return {
            getStatusColor: getStatusColor
        };
    }

    appControllers.service('VR_GenericData_DataRecordNotificationTypeSettingsService', DataRecordNotificationTypeSettingsService);

})(appControllers);