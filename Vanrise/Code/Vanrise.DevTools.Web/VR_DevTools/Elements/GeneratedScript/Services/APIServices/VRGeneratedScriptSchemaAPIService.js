(function (appControllers) {
    "use strict";
    schemaAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Devtools_ModuleConfig', 'SecurityService'];
    function schemaAPIService(BaseAPIService, UtilsService, VR_Devtools_ModuleConfig, SecurityService) {

        var controller = "Schema";

        function GetSchemasInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Devtools_ModuleConfig.moduleName, controller, "GetSchemasInfo"), { filter: filter });
        }


        return {
            GetSchemasInfo: GetSchemasInfo
        };
    }
    appControllers.service("VR_Devtools_SchemaAPIService", schemaAPIService);

})(appControllers);