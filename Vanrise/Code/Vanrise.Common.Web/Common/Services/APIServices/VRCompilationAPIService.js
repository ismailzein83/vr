//(function (appControllers) {

//    "use strict";
//    compilationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

//    function compilationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
//        var controllerName = 'VRCompilationController';

//        function ExportCompilationResult(errors) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "ExportCompilationResult"), errors, {
//                returnAllResponseParameters: true,
//                responseTypeAsBufferArray: true
//            });
//        }

//        return ({
//            ExportCompilationResult: ExportCompilationResult,
//        });
//    }

//    appControllers.service('VRCommon_CompilationAPIService', compilationAPIService);

//})(appControllers);