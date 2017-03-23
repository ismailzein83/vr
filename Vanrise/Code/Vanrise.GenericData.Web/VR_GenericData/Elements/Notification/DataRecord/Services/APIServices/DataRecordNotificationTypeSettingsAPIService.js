(function (appControllers) {

    'use strict';

    DataRecordNotificationTypeSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function DataRecordNotificationTypeSettingsAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {

        var controllerName = 'DataRecordNotificationTypeSettings';

        function GetNotificationGridColumnAttributes(notificationTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetNotificationGridColumnAttributes"), {
                notificationTypeId: notificationTypeId
            });
        }

        function GetNotificationDataRecordFieldsInfo(notificationTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, "GetNotificationDataRecordFieldsInfo"), {
                notificationTypeId: notificationTypeId
            });
        }


        return {
            GetNotificationGridColumnAttributes: GetNotificationGridColumnAttributes,
            GetNotificationDataRecordFieldsInfo: GetNotificationDataRecordFieldsInfo
        };
    }

    appControllers.service('VR_GenericData_DataRecordNotificationTypeSettingsAPIService', DataRecordNotificationTypeSettingsAPIService);

})(appControllers);