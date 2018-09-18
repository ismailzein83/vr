(function (appControllers) {

    "use strict";

    VRReportGenerationService.$inject = ['VRModalService', 'VRUIUtilsService'];

    function VRReportGenerationService(VRModalService, VRUIUtilsService) {
        var drillDownDefinitions = [];
        function addVRReportGeneration(onVRReportGenerationAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onVRReportGenerationAdded = onVRReportGenerationAdded;
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

        function testGenerateVRReportGeneration(currentReportGenerationInfo, context) {
            var settings = {};
            var parameters = {
                currentReportGenerationInfo: currentReportGenerationInfo,
                context: context
            };

            VRModalService.showModal('/Client/Modules/Analytic/Views/VRReportGeneration/VRReportGenerationGenerator.html', parameters, settings);
        }
        
        function getEntityUniqueName() {
            return "VR_Analytic_ReportGeneration";
        }

        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addDrillDownDefinition: addDrillDownDefinition,
            getDrillDownDefinition:getDrillDownDefinition,
            getEntityUniqueName:getEntityUniqueName,
            addVRReportGeneration: addVRReportGeneration,
            editVRReportGeneration: editVRReportGeneration,
            generateVRReportGeneration: generateVRReportGeneration,
            testGenerateVRReportGeneration: testGenerateVRReportGeneration
        };
    }

    appControllers.service('VR_Analytic_ReportGenerationService', VRReportGenerationService);

})(appControllers);