
(function (appControllers) {

    "use strict";

    VRReportGenerationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

    function VRReportGenerationAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

        var controllerName = "VRReportGeneration";


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

        return ({
            GetFilteredVRReportGenerations: GetFilteredVRReportGenerations,
            GetVRReportGeneration: GetVRReportGeneration,
            AddVRReportGeneration: AddVRReportGeneration,
            UpdateVRReportGeneration: UpdateVRReportGeneration,
            GetVRReportGenerationsInfo: GetVRReportGenerationsInfo,
            GetReportActionTemplateConfigs: GetReportActionTemplateConfigs
        });
    }

    appControllers.service('VR_Analytic_ReportGenerationAPIService', VRReportGenerationAPIService);
})(appControllers);