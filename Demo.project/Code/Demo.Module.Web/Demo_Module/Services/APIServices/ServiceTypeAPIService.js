(function (appControllers) {

    "use strict";
    serviceTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function serviceTypeAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {
       
        function GetServiceTypesInfo(nameFilter, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "ServiceType", "GetServiceTypesInfo"));
        }

        
        return ({
            GetServiceTypesInfo: GetServiceTypesInfo
        });
    }

    appControllers.service('ServiceTypeAPIService', serviceTypeAPIService);

})(appControllers);