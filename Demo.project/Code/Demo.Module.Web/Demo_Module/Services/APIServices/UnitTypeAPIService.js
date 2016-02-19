(function (appControllers) {

    "use strict";
    unitTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function unitTypeAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {
       
        function GetUnitTypesInfo(nameFilter, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "UnitType", "GetUnitTypesInfo"));
        }

        
        return ({
            GetUnitTypesInfo: GetUnitTypesInfo
        });
    }

    appControllers.service('UnitTypeAPIService', unitTypeAPIService);

})(appControllers);