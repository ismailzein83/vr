(function (appControllers) {

    "use strict";

    VRReportGenerationService.$inject = ['VRModalService', 'VRUIUtilsService'];

    function VRReportGenerationService(VRModalService, VRUIUtilsService) {

        function addVRReportGeneration(onVRReportGenerationAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRReportGenerationAdded = onVRReportGenerationAdded
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReportGeneration/VRReportGenerationEditor.html', null, settings);
        };

        function editVRReportGeneration(reportId, onVRReportGenerationUpdated) {
            var settings = {};

            var parameters = {
                reportId: reportId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRReportGenerationUpdated = onVRReportGenerationUpdated;
            };
            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReportGeneration/VRReportGenerationEditor.html', parameters, settings);
        }
        function generateVRReportGeneration(reportId) {
            var settings = {};

            var parameters = {
                reportId: reportId,
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReportGeneration/VRReportGenerationGenerator.html', parameters, settings);
        }
        

        return {
            addVRReportGeneration: addVRReportGeneration,
            editVRReportGeneration: editVRReportGeneration,
            generateVRReportGeneration: generateVRReportGeneration
        };
    }

    appControllers.service('VR_Analytic_ReportGenerationService', VRReportGenerationService);

})(appControllers);