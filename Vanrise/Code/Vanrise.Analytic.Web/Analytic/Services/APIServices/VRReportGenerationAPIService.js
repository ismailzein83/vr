
//(function (appControllers) {

//    "use strict";

//    VRReportGenerationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Analytic_ModuleConfig'];

//    function VRReportGenerationAPIService(BaseAPIService, UtilsService, VR_Analytic_ModuleConfig) {

//        var controllerName = "VRReport";


//        function GetFilteredVRReportGenerations(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetFilteredVRReportGenerations'), input);
//        }

//        function GetVRReportGeneration(vRReportId) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'GetVRReportGeneration'), {
//                vRReportId: vRReportId
//            });
//        }

//        function AddVRReportGeneration(vRReportItem) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'AddVRReportGeneration'), dataAnalysisDefinitionItem);
//        }

//        function UpdateVRReportGeneration(vRReportItem) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, 'UpdateVRReportGeneration'), dataAnalysisDefinitionItem);
//        }
      
//        function GetVRReportGenerationsInfo(filter) {
//            return BaseAPIService.get(UtilsService.getServiceURL(VR_Analytic_ModuleConfig.moduleName, controllerName, "GetVRReportGenerationsInfo"), {
//                filter: filter
//            });
//        }

//        return ({
//            GetFilteredVRReports: GetFilteredVRReports,
//            GetVRReport: GetVRReport,
//            AddVRReport: AddVRReport,
//            UpdateVRReport: UpdateVRReport,
//            GetVRReportsInfo: GetVRReportsInfo
//        });
//    }

//    appControllers.service('VR_Analytic_VRReportGenerationAPIService', VRReportAPIService);
//})(appControllers);