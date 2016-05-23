
app.service('VRCommon_SettingsService', ['VRModalService',
    function (VRModalService) {


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

        return ({
            editSettings: editSettings
        });
    }]);
