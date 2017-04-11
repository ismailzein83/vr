(function (appControllers) {

    'use strict';

    AccountBalanceNotificationTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig'];

    function AccountBalanceNotificationTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig) {

        var controllerName = 'AccountBalanceNotificationType';

        function GetAccountBalanceNotificationTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountBalanceNotificationTypeExtendedSettingsConfigs"));
        }

        function GetAccountColumnHeader(notificationTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountColumnHeader"), {
                notificationTypeId: notificationTypeId
            });
        }


        return {
            GetAccountBalanceNotificationTypeExtendedSettingsConfigs: GetAccountBalanceNotificationTypeExtendedSettingsConfigs,
            GetAccountColumnHeader: GetAccountColumnHeader
        };
    }

    appControllers.service('VR_AccountBalance_AccountBalanceNotificationTypeAPIService', AccountBalanceNotificationTypeAPIService);

})(appControllers);