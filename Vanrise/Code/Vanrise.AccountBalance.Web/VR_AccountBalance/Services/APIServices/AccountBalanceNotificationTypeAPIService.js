(function (appControllers) {

    'use strict';

    AccountBalanceNotificationTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig'];

    function AccountBalanceNotificationTypeAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig) {

        var controllerName = 'AccountBalanceNotificationType';

        function GetAccountBalanceNotificationTypeExtendedSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountBalanceNotificationTypeExtendedSettingsConfigs"));
        }

        function GetAccountBalanceNotificationTypeSettings(notificationTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountBalanceNotificationTypeSettings"), {
                notificationTypeId: notificationTypeId
            });
        }


        return {
            GetAccountBalanceNotificationTypeExtendedSettingsConfigs: GetAccountBalanceNotificationTypeExtendedSettingsConfigs,
            GetAccountBalanceNotificationTypeSettings: GetAccountBalanceNotificationTypeSettings
        };
    }

    appControllers.service('VR_AccountBalance_AccountBalanceNotificationTypeAPIService', AccountBalanceNotificationTypeAPIService);

})(appControllers);