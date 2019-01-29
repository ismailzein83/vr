(function (appControllers) {

    "use strict";
    rdbDataTypeInfoAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function rdbDataTypeInfoAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'RDBDataType';

        function GetRDBDataTypeInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRDBDataTypeInfo"), {}, { useCache: true });
        }

        return {
            GetRDBDataTypeInfo: GetRDBDataTypeInfo
        };
    }

    appControllers.service('VRCommon_RDBDataTypeAPIService', rdbDataTypeInfoAPIService);

})(appControllers);