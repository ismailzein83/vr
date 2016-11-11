(function (appControllers) {

    'use strict';

    ZoneServiceConfigService.$inject = ['UtilsService', 'VRModalService'];

    function ZoneServiceConfigService(UtilsService, VRModalService) {
        return ({
            addZoneServiceConfig: addZoneServiceConfig,
            editZoneServiceConfig: editZoneServiceConfig
        });

        function addZoneServiceConfig(onZoneServiceConfigAdded) {
            var settings = {
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onZoneServiceConfigAdded = onZoneServiceConfigAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }

        function editZoneServiceConfig(zoneServiceConfigId, onZoneServiceConfigUpdated) {
            var settings = {
            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onZoneServiceConfigUpdated = onZoneServiceConfigUpdated;
            };
            var parameters = {
                zoneServiceConfigId: zoneServiceConfigId
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_BE_ZoneServiceConfigService', ZoneServiceConfigService);

})(appControllers);
