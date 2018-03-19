(function (appControllers) {

    "use strict";

    qualityConfigurationService.$inject = ['UtilsService', 'VRModalService'];

    function qualityConfigurationService(UtilsService, VRModalService) {

        function addRouteRuleQualityConfiguration(onQualityConfigurationAdded, qualityConfigurationNames) {

            var modalParameters = {
                qualityConfigurationNames: qualityConfigurationNames
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQualityConfigurationAdded = onQualityConfigurationAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationRuntime/Templates/RouteRuleQualityConfigurationEditor.html', modalParameters, modalSettings);
        }

        function editRouteRuleQualityConfiguration(onQualityConfigurationUpdated, qualityConfigurationEntity, qualityConfigurationNames) {

            var modalParameters = {
                qualityConfigurationEntity: qualityConfigurationEntity,
                qualityConfigurationNames: qualityConfigurationNames
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQualityConfigurationUpdated = onQualityConfigurationUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Directives/QualityConfiguration/QualityConfigurationRuntime/Templates/RouteRuleQualityConfigurationEditor.html', modalParameters, modalSettings);
        }


        return ({
            addRouteRuleQualityConfiguration: addRouteRuleQualityConfiguration,
            editRouteRuleQualityConfiguration: editRouteRuleQualityConfiguration
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationService', qualityConfigurationService);
})(appControllers);