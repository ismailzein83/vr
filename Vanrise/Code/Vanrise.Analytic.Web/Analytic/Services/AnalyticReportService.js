(function (appControllers) {

    'use strict';

    AnalyticReportService.$inject = ['VRModalService'];

    function AnalyticReportService(VRModalService) {
        return ({
            addAnalyticReport: addAnalyticReport,
            editAnalyticReport: editAnalyticReport,
        });

        function addAnalyticReport(onAnalyticReportAdded,configId) {
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticReportAdded = onAnalyticReportAdded;
            };
            var parameters = {
                configId: configId
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportEditor.html', parameters, modalSettings);
        }

        function editAnalyticReport(analyticReportId, onAnalyticReportUpdated, configId) {
            var modalParameters = {
                analyticReportId: analyticReportId,
                configId: configId
            };
            var modalSettings = {};

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onAnalyticReportUpdated = onAnalyticReportUpdated;
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/GenericAnalytic/Definition/AnalyticReportEditor.html', modalParameters, modalSettings);
        }

    };

    appControllers.service('VR_Analytic_AnalyticReportService', AnalyticReportService);

})(appControllers);
