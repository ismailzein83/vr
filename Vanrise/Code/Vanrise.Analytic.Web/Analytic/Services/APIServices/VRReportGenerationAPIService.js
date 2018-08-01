
(function (appControllers) {

    "use strict";

    VRReportGenerationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function VRReportGenerationAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "VRReportGeneration";

        function GetVRReportGenerationHistoryDetailbyHistoryId(vRReportGenerationHistoryId) {

            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetVRReportGenerationHistoryDetailbyHistoryId'), {
                vRReportGenerationHistoryId: vRReportGenerationHistoryId
            });
        }
        function GetFilteredVRReportGenerations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredVRReportGenerations'), input);
        }

        function GetVRReportGeneration(reportId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetVRReportGeneration'), {
                reportId: reportId
            });
        }

        function AddVRReportGeneration(vRReportGenerationItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddVRReportGeneration'), vRReportGenerationItem);
        }

        function UpdateVRReportGeneration(vRReportGenerationItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateVRReportGeneration'), vRReportGenerationItem);
        }

        function GetVRReportGenerationsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetVRReportGenerationsInfo"), {
                filter: filter
            });
        }
        function GetReportActionTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetReportActionTemplateConfigs'));
        }
        function DoesUserHaveManageAccess() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'DoesUserHaveManageAccess'));
        }

        return ({
            GetFilteredVRReportGenerations: GetFilteredVRReportGenerations,
            GetVRReportGeneration: GetVRReportGeneration,
            AddVRReportGeneration: AddVRReportGeneration,
            UpdateVRReportGeneration: UpdateVRReportGeneration,
            GetVRReportGenerationsInfo: GetVRReportGenerationsInfo,
            GetReportActionTemplateConfigs: GetReportActionTemplateConfigs,
            GetVRReportGenerationHistoryDetailbyHistoryId: GetVRReportGenerationHistoryDetailbyHistoryId,
            DoesUserHaveManageAccess: DoesUserHaveManageAccess
        });
    }

    appControllers.service('VR_Analytic_ReportGenerationAPIService', VRReportGenerationAPIService);
})(appControllers);