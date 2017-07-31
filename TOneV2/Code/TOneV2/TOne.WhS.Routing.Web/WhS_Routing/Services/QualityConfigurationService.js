﻿(function (appControllers) {

    "use strict";
    qualityConfigurationService.$inject = ['UtilsService', 'VRModalService'];

    function qualityConfigurationService(UtilsService, VRModalService) {

        function addQualityConfiguration(onQualityConfigurationAdded) {

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQualityConfigurationAdded = onQualityConfigurationAdded;
            };
            VRModalService.showModal('/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/QualityConfigurationEditor.html', undefined, modalSettings);
        }

        function editQualityConfiguration(editQualityConfigurationObject, onQualityConfigurationUpdated) {
            var modalParameters = {
                editQualityConfigurationObject: editQualityConfigurationObject
            };

            var modalSettings = {};
            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onQualityConfigurationUpdated = onQualityConfigurationUpdated;
            };

            VRModalService.showModal('/Client/Modules/WhS_Routing/Directives/RouteSettings/Templates/QualityConfigurationEditor.html', modalParameters, modalSettings);
        }


        return ({
            addQualityConfiguration: addQualityConfiguration,
            editQualityConfiguration: editQualityConfiguration
        });
    }

    appControllers.service('WhS_Routing_QualityConfigurationService', qualityConfigurationService);
})(appControllers);