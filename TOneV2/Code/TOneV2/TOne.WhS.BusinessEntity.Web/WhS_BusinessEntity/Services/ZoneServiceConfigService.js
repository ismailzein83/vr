app.service('WhS_BE_ZoneServiceConfigService', ['VRModalService', 'VRNotificationService', 'UtilsService',
    function (VRModalService, VRNotificationService, UtilsService) {

        function editZoneServiceConfig(obj, onZoneServiceConfigUpdated) {
            var settings = {
                useModalTemplate: true

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForUpdateEditor(obj.Name, "Zone Service Config");
                modalScope.onZoneServiceConfigUpdated = onZoneServiceConfigUpdated;
            };
            var parameters = {
                ServiceFlag: obj.ServiceFlag
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }
        function addZoneServiceConfig(onZoneServiceConfigAdded) {
            var settings = {
                useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.title = UtilsService.buildTitleForAddEditor("Zone Service Config");
                modalScope.onZoneServiceConfigAdded = onZoneServiceConfigAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }

        

        return ({
            editZoneServiceConfig: editZoneServiceConfig,
            addZoneServiceConfig: addZoneServiceConfig
        });

    }]);

