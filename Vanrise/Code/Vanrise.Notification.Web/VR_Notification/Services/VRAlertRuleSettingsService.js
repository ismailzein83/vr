(function (appControllers) {

    'use stict';

    VRAlertRuleSettingsService.$inject = ['VRModalService', 'VRNotificationService'];

    function VRAlertRuleSettingsService(VRModalService, VRNotificationService) {
        function addAlertRuleThreshold(onAlertRuleSettingsAdded, actionExtensionType, thresholdExtensionType) {

            var settings = {};
            var parameters = {
                actionExtensionType: actionExtensionType,
                thresholdExtensionType: thresholdExtensionType
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertRuleSettingsAdded = onAlertRuleSettingsAdded
            };

            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleSettings/AlertRuleSettingsEditor.html', parameters, settings);
        };
        function editAlertRuleThreshold(thresholdActionEntity, onAlertRuleSettingsUpdated, actionExtensionType, thresholdExtensionType) {
            var settings = {};
            var parameters = {
                thresholdActionEntity: thresholdActionEntity,
                actionExtensionType: actionExtensionType,
                thresholdExtensionType: thresholdExtensionType
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onAlertRuleSettingsUpdated = onAlertRuleSettingsUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_Notification/Views/VRAlertRuleSettings/AlertRuleSettingsEditor.html', parameters, settings);
        }
        return {
            addAlertRuleThreshold: addAlertRuleThreshold,
            editAlertRuleThreshold: editAlertRuleThreshold
        };
    }

    appControllers.service('VR_Notification_AlertRuleSettingsService', VRAlertRuleSettingsService);

})(appControllers);