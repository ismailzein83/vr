(function (appControllers) {

    'use strict';

    ZoneServiceConfigService.$inject = ['UtilsService', 'VRModalService', 'VRCommon_ObjectTrackingService'];

    function ZoneServiceConfigService(UtilsService, VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addZoneServiceConfig: addZoneServiceConfig,
            editZoneServiceConfig: editZoneServiceConfig,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToZoneServiceConfig: registerObjectTrackingDrillDownToZoneServiceConfig
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

        function getEntityUniqueName() {
            return "WhS_BusinessEntity_ZoneServiceConfig";
        }

        function registerObjectTrackingDrillDownToZoneServiceConfig() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, zoneServiceConfigItem) {
                zoneServiceConfigItem.objectTrackingGridAPI = directiveAPI;

                var query = {
                    ObjectId: zoneServiceConfigItem.Entity.ZoneServiceConfigId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return zoneServiceConfigItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }


    }

    appControllers.service('WhS_BE_ZoneServiceConfigService', ZoneServiceConfigService);

})(appControllers);
