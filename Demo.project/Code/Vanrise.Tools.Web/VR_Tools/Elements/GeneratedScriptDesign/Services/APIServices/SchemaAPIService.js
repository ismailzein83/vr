(function (appControllers) {
    "use strict";
    schemaAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Tools_ModuleConfig', 'SecurityService'];
    function schemaAPIService(BaseAPIService, UtilsService, VR_Tools_ModuleConfig, SecurityService) {

        var controller = "Schema";

        function GetSchemasInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Tools_ModuleConfig.moduleName, controller, "GetSchemasInfo"), { filter: filter });
        }


        return {
            GetSchemasInfo: GetSchemasInfo
        };
    }
    appControllers.service("VR_Tools_SchemaAPIService", schemaAPIService);
    
})(appControllers);