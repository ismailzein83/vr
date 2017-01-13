
app.service('VRCommon_SettingsService', ['VRModalService','UtilsService',
    function (VRModalService, UtilsService) {


        function editSettings(settingsId, onSettingsUpdated) {
            var settings = {

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onSettingsUpdated = onSettingsUpdated;
            };
            var parameters = {
                settingsId: settingsId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Settings/SettingsEditor.html', parameters, settings);
        }
        function viewSettings(settingsId) {
            var settings = {

            };
            settings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope)
            };
            var parameters = {
                settingsId: settingsId
            };

            VRModalService.showModal('/Client/Modules/Common/Views/Settings/SettingsEditor.html', parameters, settings);
        }

        return ({
            editSettings: editSettings,
            viewSettings: viewSettings
        });
    }]);
