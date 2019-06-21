//(function (appControllers) {

//    'use strict';

//    BusinessEntityStatusHistoryAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

//    function BusinessEntityStatusHistoryAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
//        var controllerName = "BusinessEntityStatusHistory";

//        return {
//            GetFilteredBusinessEntitiesStatusHistory: GetFilteredBusinessEntitiesStatusHistory,
//        };

//        function GetFilteredBusinessEntitiesStatusHistory(input) {
//            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredBusinessEntitiesStatusHistory'), input);
//        }
//    }

//    appControllers.service('VR_GenericData_BusinessEntityStatusHistoryAPIService', BusinessEntityStatusHistoryAPIService);

//})(appControllers);