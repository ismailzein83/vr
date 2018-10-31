(function (appControllers) {
    "use strict";
    pageDefintionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function pageDefintionAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "PageDefinition";

        function GetFilteredPageDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFilteredPageDefinitions"), input);
        }

        function GetPageDefinitionById(pageDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPageDefinitionById"),
                {
                    pageDefinitionId: pageDefinitionId
                });
        }

        function UpdatePageDefinition(pageDefintion) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "UpdatePageDefinition"), pageDefintion);
        }

        function AddPageDefinition(pageDefintion) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "AddPageDefinition"), pageDefintion);
        };

        function GetPageDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPageDefinitionsInfo"), { filter: filter });
        };
        function GetFieldTypeConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetFieldTypeConfigs"));
        }
        function GetSubviewConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetSubviewConfigs"));
        }
        function GetPageDefinitionConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetPageDefinitionConfigs"));
        }
        

        return {
            GetFilteredPageDefinitions: GetFilteredPageDefinitions,
            GetPageDefinitionById: GetPageDefinitionById,
            UpdatePageDefinition: UpdatePageDefinition,
            AddPageDefinition: AddPageDefinition,
            GetPageDefinitionsInfo: GetPageDefinitionsInfo,
            GetFieldTypeConfigs: GetFieldTypeConfigs,
            GetSubviewConfigs: GetSubviewConfigs,
            GetPageDefinitionConfigs: GetPageDefinitionConfigs
          
       };
    };
    appControllers.service("Demo_Module_PageDefinitionAPIService", pageDefintionAPIService);

})(appControllers);