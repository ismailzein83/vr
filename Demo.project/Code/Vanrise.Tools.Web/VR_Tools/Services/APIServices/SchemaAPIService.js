(function (appControllers) {
    "use strict";
    schemaAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Vanrise_Tools_ModuleConfig', 'SecurityService'];
    function schemaAPIService(BaseAPIService, UtilsService, Vanrise_Tools_ModuleConfig, SecurityService) {

        var controller = "Schema";

        function GetSchemasInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Vanrise_Tools_ModuleConfig.moduleName, controller, "GetSchemasInfo"), { filter: filter });
        };


        return {
            GetSchemasInfo: GetSchemasInfo
        };
    };
    appControllers.service("Vanrise_Tools_SchemaAPIService", schemaAPIService);
    
})(appControllers);