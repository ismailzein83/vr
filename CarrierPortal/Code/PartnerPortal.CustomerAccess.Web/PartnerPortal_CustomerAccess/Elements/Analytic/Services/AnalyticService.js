
(function (appControllers) {

    'use strict';

    AnalyticService.$inject = ['VRModalService', 'UtilsService'];

    function AnalyticService(VRModalService, UtilsService) {
        return {
            addAnalyticQuery: addAnalyticQuery,
            editAnalyticQuery: editAnalyticQuery,
        };

        function addAnalyticQuery(onAnalyticQueryAdded) {
            var modalParameters = {

            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticQueryAdded = onAnalyticQueryAdded;
            };

            VRModalService.showModal('/Client/Modules/PartnerPortal_CustomerAccess/Elements/Analytic/Directives/Templates/AnalyticQueryEditor.html', modalParameters, modalSettings);
        }

        function editAnalyticQuery(analyticQueryEntity, onAnalyticQueryUpdated) {
            var modalParameters = {
                analyticQueryEntity: analyticQueryEntity
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticQueryUpdated = onAnalyticQueryUpdated;
            };

            VRModalService.showModal('/Client/Modules/PartnerPortal_CustomerAccess/Elements/Analytic/Directives/Templates/AnalyticQueryEditor.html', modalParameters, modalSettings);
        }
    }

    appControllers.service('PartnerPortal_CustomerAccess_AnalyticService', AnalyticService);

})(appControllers);