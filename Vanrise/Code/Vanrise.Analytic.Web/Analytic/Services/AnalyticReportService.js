(function (appControllers) {

    'use strict';

    AnalyticReportService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService'];

    function AnalyticReportService(VRModalService, VRCommon_ObjectTrackingService) {
        var drillDownDefinitions = [];
        return ({
            addAnalyticReport: addAnalyticReport,
            editAnalyticReport: editAnalyticReport,
            registerObjectTrackingDrillDownToAnalyticReport: registerObjectTrackingDrillDownToAnalyticReport,
            getDrillDownDefinition: getDrillDownDefinition
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

        function getEntityUniqueName() {
            return "VR_Analytic_AnalyticReport";
        }

        function registerObjectTrackingDrillDownToAnalyticReport() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, analyticReportItem) {
                analyticReportItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: analyticReportItem.Entity.AnalyticReportId,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return analyticReportItem.objectTrackingGridAPI.load(query);
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

    appControllers.service('VR_Analytic_AnalyticReportService', AnalyticReportService);

})(appControllers);
