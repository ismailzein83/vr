(function (appControllers) {

    'use strict';

    AnalyticItemConfigService.$inject = ['VRModalService'];

    function AnalyticItemConfigService(VRModalService) {
        return ({
            addItemConfig: addItemConfig,
            editItemConfig: editItemConfig,
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

    };

    appControllers.service('VR_Analytic_AnalyticItemConfigService', AnalyticItemConfigService);

})(appControllers);
