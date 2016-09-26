
(function (appControllers) {

    'use stict';

    function beReceiveDefinitionSettingsService(VRModalService, VRNotificationService) {

        function addReceiveDefinitionSettings(onReceiveDefinitionSettingsAdded, existingFields) {
            var modalSettings = {};
            var modalParameters = {
                ExistingFields: existingFields
            };
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onReceiveDefinitionSettingsAdded = onReceiveDefinitionSettingsAdded;
            };
            VRModalService.showModal('/Client/Modules/VR_BEBridge/Views/BEReceiveDefinitionSettings/BEReceiveDefinitionSettingEditor.html', modalParameters, modalSettings);
        };

        function editReceiveDefinitionSettings(settingsField, onReceiveDefinitionSettingsUpdated, existingFields) {
            var modalSettings = {};

            var modalParameters = {
                ExistingFields: existingFields,
                SettingsField: settingsField
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onReceiveDefinitionSettingsUpdated = onReceiveDefinitionSettingsUpdated;
            };
            VRModalService.showModal('/Client/Modules/VR_BEBridge/Views/BEReceiveDefinitionSettings/BEReceiveDefinitionSettingEditor.html', modalParameters, modalSettings);
        }

        function deleteReceiveDefinitionSettings($scope, settingsFieldObj, onSettingsFieldDeleted) {
            VRNotificationService.showConfirmation()
                .then(function (response) {
                    if (response) {
                        onSettingsFieldDeleted(settingsFieldObj);
                    }
                });
        }

        return {
            addReceiveDefinitionSettings: addReceiveDefinitionSettings,
            editReceiveDefinitionSettings: editReceiveDefinitionSettings,
            deleteReceiveDefinitionSettings: deleteReceiveDefinitionSettings
        };
    }

    beReceiveDefinitionSettingsService.$inject = ['VRModalService', 'VRNotificationService'];
    appControllers.service('VR_BEBridge_BEReceiveDefinitionSettingsService', beReceiveDefinitionSettingsService);

})(appControllers);