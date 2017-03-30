(function (appControllers) {

    'use strict';

    AnalyticItemConfigService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'VR_Analytic_AnalyticTypeEnum'];

    function AnalyticItemConfigService(VRModalService, VRCommon_ObjectTrackingService, VR_Analytic_AnalyticTypeEnum) {
        var drillDownDefinitions = [];
        return ({
            addItemConfig: addItemConfig,
            editItemConfig: editItemConfig,
            registerObjectTrackingDrillDownToAnalyticItemConfig: registerObjectTrackingDrillDownToAnalyticItemConfig,
            getDrillDownDefinition: getDrillDownDefinition,
        });



        function addItemConfig(onAnalyticItemConfigAdded, analyticTableId, itemConfigType) {
            var modalSettings = {
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticItemConfigAdded = onAnalyticItemConfigAdded;
            };

            var parameters = {
                itemconfigType: itemConfigType,
                analyticTableId: analyticTableId
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemConfigEditor.html', parameters, modalSettings);
        }


        function editItemConfig(analyticItemConfigId, onAnalyticItemConfigUpdated, analyticTableId, itemConfigType) {
            var modalParameters = {
                    analyticItemConfigId: analyticItemConfigId,
            itemconfigType: itemConfigType,
            analyticTableId: analyticTableId
        };

            var modalSettings = {
        };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticItemConfigUpdated = onAnalyticItemConfigUpdated;
        };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemConfigEditor.html', modalParameters, modalSettings);
        }


        function getEntityUniqueName(AnalyticItemConfigType) {
            return "VR_Analytic_AnalyticItemConfig_" + AnalyticItemConfigType;
        }

        function registerObjectTrackingDrillDownToAnalyticItemConfig() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";

           
            drillDownDefinition.loadDirective = function (directiveAPI, analyticItemConfigItem) {
               
                analyticItemConfigItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: analyticItemConfigItem.Entity.AnalyticItemConfigId,
                    EntityUniqueName: getEntityUniqueName(analyticItemConfigItem.Entity.ItemType),

                };
                return analyticItemConfigItem.objectTrackingGridAPI.load(query);
            };

            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {

            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

    };

    appControllers.service('VR_Analytic_AnalyticItemConfigService', AnalyticItemConfigService);

})(appControllers);
