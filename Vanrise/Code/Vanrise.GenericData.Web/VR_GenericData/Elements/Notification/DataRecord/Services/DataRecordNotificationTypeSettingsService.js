(function (appControllers) {

    'use strict';

    DataRecordNotificationTypeSettingsService.$inject = ['VRModalService', 'VR_Notification_NotificationStatusEnum', 'LabelColorsEnum', ];

    function DataRecordNotificationTypeSettingsService(VRModalService, VR_Notification_NotificationStatusEnum, LabelColorsEnum) {

        function getStatusColor(status) {

            if (status === VR_Notification_NotificationStatusEnum.New.value) return LabelColorsEnum.Primary.color;
            if (status === VR_Notification_NotificationStatusEnum.Executing.value) return LabelColorsEnum.Info.color;
            if (status === VR_Notification_NotificationStatusEnum.Completed.value) return LabelColorsEnum.Success.color;
            if (status === VR_Notification_NotificationStatusEnum.Cleared.value) return LabelColorsEnum.Error.color;
            if (status === VR_Notification_NotificationStatusEnum.Suspended.value) return LabelColorsEnum.Warning.color;

            return LabelColorsEnum.Info.color;
        };

        return {
            getStatusColor: getStatusColor
        };
    }

    appControllers.service('VR_GenericData_DataRecordNotificationTypeSettingsService', DataRecordNotificationTypeSettingsService);

})(appControllers);