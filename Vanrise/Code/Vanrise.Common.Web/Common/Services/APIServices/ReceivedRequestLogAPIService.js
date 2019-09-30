//(function (appControllers) {

//    "use strict";
//    ReceivedRequestLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

//    function ReceivedRequestLogAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

//        var controllerName = 'ReceivedRequestLog';

//        function GetReceivedRequestLogFilterModuleConfigs() {
//            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetReceivedRequestLogFilterModuleConfigs"));
//        }

//        return ({
//            GetReceivedRequestLogFilterModuleConfigs: GetReceivedRequestLogFilterModuleConfigs

//        });
//    }

//    appControllers.service('VRCommon_ReceivedRequestLogAPIService', ReceivedRequestLogAPIService);

//})(appControllers);