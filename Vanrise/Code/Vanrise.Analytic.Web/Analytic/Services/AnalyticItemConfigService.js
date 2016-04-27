(function (appControllers) {

    'use strict';

    AnalyticItemConfigService.$inject = ['VRModalService'];

    function AnalyticItemConfigService(VRModalService) {
        return ({
            addItemConfig: addItemConfig,
            editItemConfig: editItemConfig,
        });

        function addItemConfig(onAnalyticItemConfigAdded) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticItemConfigAdded = onAnalyticItemConfigAdded;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemConfigEditor.html', null, modalSettings);
        }

        function editItemConfig(analyticItemConfigId, onAnalyticItemConfigUpdated) {
            var modalParameters = {
                analyticItemConfigId: analyticItemConfigId
            };

            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticItemConfigUpdated = onAnalyticItemConfigUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticItemConfigEditor.html', modalParameters, modalSettings);
        }

    };

    appControllers.service('VR_Analytic_AnalyticItemConfigService', AnalyticItemConfigService);

})(appControllers);
