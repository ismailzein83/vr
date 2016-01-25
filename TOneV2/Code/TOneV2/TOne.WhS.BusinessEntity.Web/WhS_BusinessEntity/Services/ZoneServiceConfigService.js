﻿(function (appControllers) {

    'use stict';

    ZoneServiceConfigService.$inject = ['UtilsService', 'VRModalService'];

    function ZoneServiceConfigService(UtilsService, VRModalService) {
        return ({
            addZoneServiceConfig: addZoneServiceConfig,
            editZoneServiceConfig: editZoneServiceConfig
        });

        function addZoneServiceConfig(onZoneServiceConfigAdded) {
            var settings = {
                // useModalTemplate: true

            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onZoneServiceConfigAdded = onZoneServiceConfigAdded;
            };
            var parameters = {};

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }

        function editZoneServiceConfig(obj, onZoneServiceConfigUpdated) {
            var settings = {
                //useModalTemplate: true

            };
            settings.onScopeReady = function (modalScope) {
                modalScope.onZoneServiceConfigUpdated = onZoneServiceConfigUpdated;
            };
            var parameters = {
                ServiceFlag: obj.ServiceFlag
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/ZoneServiceConfig/ZoneServiceConfigEditor.html', parameters, settings);
        }
    }

    appControllers.service('WhS_BE_ZoneServiceConfigService', ZoneServiceConfigService);

})(appControllers);
